﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

using BEditor;
using BEditor.Data;
using BEditor.Data.Property;
using BEditor.Properties;
using BEditor.ViewModels;
using BEditor.Views;
using BEditor.Views.MessageContent;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;

using Reactive.Bindings;

namespace BEditor.Models
{
    public class ProjectModel
    {
        public static readonly ProjectModel Current = new();

        private ProjectModel()
        {
            SaveAs.Where(_ => AppData.Current.Project is not null)
                .Select(_ => AppData.Current.Project)
                .Subscribe(async p =>
                {
                    await using var prov = AppData.Current.Services.BuildServiceProvider();
                    var dialog = prov.GetService<IFileDialogService>();

                    if (dialog is null) throw new InvalidOperationException();

                    var record = new SaveFileRecord
                    {
                        DefaultFileName = (p!.Name is not null) ? p.Name + ".bedit" : "新しいプロジェクト.bedit",
                        Filters =
                        {
                            new(Resources.ProjectFile, new FileExtension[] { new("bedit") }),
                            new(Resources.JsonFile, new FileExtension[] { new("json") }),
                        }
                    };

                    var mode = SerializeMode.Binary;

                    if (dialog.ShowSaveFileDialog(record))
                    {
                        if (Path.GetExtension(record.FileName) is ".json")
                        {
                            mode = SerializeMode.Json;
                        }

                        p.Save(record.FileName, mode);
                    }
                });

            Save.Where(_ => AppData.Current.Project is not null)
                .Select(_ => AppData.Current.Project)
                .Subscribe(async p =>
                {
                    MainWindowViewModel.Current.IsLoading.Value = true;
                    await Task.Run(() =>
                    {
                        p!.Save();

                        MainWindowViewModel.Current.IsLoading.Value = false;
                    });
                });

            Open.Select(_ => AppData.Current).Subscribe(async app =>
            {
                var dialog = new OpenFileDialog()
                {
                    Filter = $"{Resources.ProjectFile}|*.bedit|{Resources.BackupFile}|*.backup|{Resources.JsonFile}|*.json",
                    RestoreDirectory = true
                };

                if (dialog.ShowDialog() ?? false)
                {
                    NoneDialog? ndialog = null;
                    try
                    {
                        var loading = new Loading()
                        {
                            IsIndeterminate = { Value = true }
                        };
                        ndialog = new NoneDialog(loading)
                        {
                            Owner = App.Current.MainWindow
                        };
                        ndialog.Show();

                        await DirectOpen(dialog.FileName);
                    }
                    catch
                    {
                        Debug.Assert(false);
                        await using var prov = AppData.Current.Services.BuildServiceProvider();

                        prov.GetService<IMessage>()?.Snackbar(string.Format(Resources.FailedToLoad, "Project"));
                    }
                    finally
                    {
                        ndialog?.Close();
                    }
                }
            });

            Close.Select(_ => AppData.Current)
                .Where(app => app.Project is not null)
                .Subscribe(app =>
            {
                app.Project?.Unload();
                app.Project = null;
                app.AppStatus = Status.Idle;
            });

            Create.Subscribe(_ =>
            {
                AppData.Current.Project?.Unload();
                CreateEvent?.Invoke(this, EventArgs.Empty);
            });
        }

        public event EventHandler? CreateEvent;

        public ReactiveCommand SaveAs { get; } = new();
        public ReactiveCommand Save { get; } = new();
        public ReactiveCommand Open { get; } = new();
        public ReactiveCommand Close { get; } = new();
        public ReactiveCommand Create { get; } = new();

        public static async ValueTask DirectOpen(string filename)
        {
            var app = AppData.Current;
            app.Project?.Unload();
            var project = Project.FromFile(filename, app);

            if (project is null) return;

            await Task.Run(() =>
            {
                project.Load();

                app.Project = project;
                app.AppStatus = Status.Edit;

                Settings.Default.MostRecentlyUsedList.Remove(filename);
                Settings.Default.MostRecentlyUsedList.Add(filename);
            });
        }
    }
}
