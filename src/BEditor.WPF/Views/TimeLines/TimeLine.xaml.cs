﻿using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

using BEditor.Command;
using BEditor.Data;
using BEditor.Models;
using BEditor.Models.Extension;
using BEditor.ViewModels;
using BEditor.ViewModels.TimeLines;

using MaterialDesignThemes.Wpf;

using Microsoft.Xaml.Behaviors;

using Resource = BEditor.Properties.Resources;

namespace BEditor.Views.TimeLines
{
    /// <summary>
    /// TimeLine.xaml の相互作用ロジック
    /// </summary>
    public partial class TimeLine : UserControl
    {
        private readonly Scene _Scene;
        private readonly TimeLineViewModel _ViewModel;

        public TimeLine(Scene scene)
        {
            _Scene = scene;
            DataContext = _ViewModel = scene.GetCreateTimeLineViewModel();

            InitializeComponent();

            ContextMenu CreateMenu(int layer)
            {
                ContextMenu contextMenu = new ContextMenu();

                #region 削除

                MenuItem Delete = new MenuItem();

                var deletemenu = new VirtualizingStackPanel() { Orientation = Orientation.Horizontal };
                deletemenu.Children.Add(new PackIcon() { Kind = PackIconKind.Delete, Margin = new Thickness(5, 0, 5, 0) });
                deletemenu.Children.Add(new TextBlock() { Text = Resource.Remove, Margin = new Thickness(20, 0, 5, 0) });
                Delete.Header = deletemenu;

                contextMenu.Items.Add(Delete);

                Delete.Click += (_, _) =>
                {
                    scene.RemoveLayer(layer).Execute();
                };

                #endregion

                return contextMenu;
            }

            var clipMenu = new MenuItem()
            {
                Header = Resource.AddClip
            };
            foreach (var objmetadata in ObjectMetadata.LoadedObjects)
            {
                var menu = new MenuItem
                {
                    DataContext = objmetadata,
                    Command = _ViewModel.AddClip
                };

                menu.SetBinding(MenuItem.CommandParameterProperty, new Binding());
                menu.SetBinding(HeaderedItemsControl.HeaderProperty, new Binding("Name") { Mode = BindingMode.OneTime });
                clipMenu.Items.Add(menu);
            }
            TimelineMenu.Items.Insert(0, clipMenu);
            TimelineMenu.Items.Insert(1, new Separator());


            //レイヤー名追加for
            for (int l = 1; l < 100; l++)
            {
                Binding binding2 = new Binding("TrackHeight");

                Grid track = new Grid();

                Grid grid = new Grid()
                {
                    Margin = new Thickness(0, 1, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    AllowDrop = true,
                    VerticalAlignment = VerticalAlignment.Top
                };

                grid.SetValue(AttachmentProperty.IntProperty, l);
                grid.SetBinding(WidthProperty, new Binding("TrackWidth.Value") { Mode = BindingMode.OneWay });
                grid.SetResourceReference(BackgroundProperty, "MaterialDesignCardBackground");

                #region Eventの設定

                var triggers = Interaction.GetTriggers(grid);

                //MouseDown
                triggers.Add(CommandTool.CreateEvent("MouseDown", _ViewModel.LayerSelectCommand, grid));

                //PreviewDrop
                triggers.Add(CommandTool.CreateEvent("PreviewDrop", _ViewModel.LayerDropCommand, EventArgsConverter.Converter, grid));

                //MouseMove
                triggers.Add(CommandTool.CreateEvent("MouseMove", _ViewModel.LayerMoveCommand, grid));

                //PreviewDragOver
                triggers.Add(CommandTool.CreateEvent("PreviewDragOver", _ViewModel.LayerDragOverCommand, EventArgsConverter.Converter, grid));

                //MouseLeftButtonDown
                //triggers.Add(CommandTool.CreateEvent("MouseLeftButtonDown", TimeLineViewModel.TimeLineMouseLeftDownCommand, new MousePositionConverter(), grid));

                #endregion

                grid.SetBinding(HeightProperty, binding2);


                track.Children.Add(grid);
                Layer.Children.Add(track);


                Binding binding = new Binding("ActualHeight")
                {
                    Source = track
                };

                #region Labelの追加

                Grid row2 = new Grid
                {
                    ContextMenu = CreateMenu(l),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Width = 200
                };

                row2.SetBinding(HeightProperty, binding);

                var toggle = new ToggleButton()
                {
                    Style = (Style)Resources["TimelineHideShowToggleButton"],
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    Content = l,
                    Width = 200
                };
                toggle.SetBinding(HeightProperty, binding2);
                row2.Children.Add(toggle);

                toggle.Click += (s, _) =>
                {
                    var toggle = (ToggleButton)s;
                    var l = (int)toggle.Content;

                    if (!(bool)toggle.IsChecked!)
                    {
                        _Scene.HideLayer.Remove(l);
                    }
                    else
                    {
                        _Scene.HideLayer.Add(l);
                    }

                    _Scene.Parent!.PreviewUpdate();
                };

                if (_Scene.HideLayer.Exists(x => x == l))
                {
                    toggle.IsChecked = true;
                }

                LayerLabel.Children.Add(row2);

                #endregion
            }

            ScrollLabel.ScrollToVerticalOffset(_Scene.TimeLineVerticalOffset);
            ScrollLine.ScrollToVerticalOffset(_Scene.TimeLineVerticalOffset);
            ScrollLine.ScrollToHorizontalOffset(_Scene.TimeLineHorizonOffset);

            var linetrigger = Interaction.GetTriggers(ScrollLine);
            linetrigger.Add(CommandTool.CreateEvent("ScrollChanged", _ViewModel.ScrollLineCommand));

            var labeltrigger = Interaction.GetTriggers(ScrollLabel);
            labeltrigger.Add(CommandTool.CreateEvent("ScrollChanged", _ViewModel.ScrollLabelCommand));

            _ViewModel.ResetScale = (zoom, max, rate) => AddScale(zoom, max, rate);
            _ViewModel.ClipLayerMoveCommand = (data, layer) =>
            {
                var vm = data.GetCreateClipViewModel();
                var from = vm.Row;
                vm.Row = layer;

                App.Current.Dispatcher.Invoke(() =>
                {
                    Grid togrid = (Grid)Layer.Children[layer];

                    var ui = data.GetCreateClipView();
                    (ui.Parent as Grid)?.Children?.Remove(ui);

                    togrid.Children.Add(ui);
                });
            };
            _ViewModel.GetLayerMousePosition = () => Mouse.GetPosition(Layer);

            _ViewModel.ViewLoaded = true;
            _ViewModel.TimeLineLoaded(list =>
            {
                for (int index = 0; index < list.Count; index++)
                {
                    var info = list[index];


                    Grid grid = (Grid)Layer.Children[info.Layer];

                    grid.Children.Add(info.GetCreateClipView());
                }
                Layer.Focus();
            });

            _Scene.Datas.CollectionChanged += (s, e) =>
            {
                Dispatcher.Invoke(() =>
                {
                    if (e.Action == NotifyCollectionChangedAction.Add)
                    {
                        var item = _Scene.Datas[e.NewStartingIndex];

                        Grid grid = (Grid)Layer.Children[item.Layer];

                        grid.Children.Add(item.GetCreateClipView());
                    }
                    else if (e.Action == NotifyCollectionChangedAction.Remove)
                    {
                        var item = e.OldItems![0];

                        if (item is ClipElement clip)
                        {
                            var ui = clip.GetCreateClipView();
                            (ui.Parent as Grid)?.Children?.Remove(ui);
                        }
                    }
                });
            };
        }


        #region Scrollbarの移動量を変更

        private void ScrollLine_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scrollviewer = (ScrollViewer)sender;

            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                if (!(_Scene.TimeLineZoom > 200 || _Scene.TimeLineZoom < 1))
                {
                    var offset = scrollviewer.HorizontalOffset;
                    var frame = _ViewModel.ToFrame(offset);
                    _Scene.TimeLineZoom += (e.Delta / 120) * 5;

                    scrollviewer.ScrollToHorizontalOffset(_ViewModel.ToPixel(frame));
                }
            }
            else
            {
                if (e.Delta > 0)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        scrollviewer.LineLeft();
                    }
                }
                else
                {
                    for (int i = 0; i < 4; i++)
                    {
                        scrollviewer.LineRight();
                    }
                }
            }

            e.Handled = true;
        }

        #endregion

        /// <summary>
        /// 目盛りを追加するメソッド
        /// </summary>
        /// <param name="zoom">拡大率 1 - 200</param>
        /// <param name="max">最大フレーム</param>
        /// <param name="rate">フレームレート</param>
        private void AddScale(float zoom, int max, int rate)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                int top = 16;//15
                double ToPixel(int frame)
                {
                    return Setting.WidthOf1Frame * (zoom / 200) * frame;
                }

                double SecToPixel(float sec)
                {
                    return ToPixel((int)(sec * rate));
                }

                double MinToPixel(float min)
                {
                    return SecToPixel(min * 60);
                }

                scale.Children.Clear();
                //max 1000
                if (zoom <= 200 && zoom >= 100)
                {
                    //sは秒数
                    for (int s = 0; s < (max / rate); s++)
                    {
                        //一秒毎
                        Border border = new Border
                        {
                            Width = 1,
                            HorizontalAlignment = HorizontalAlignment.Left,
                            VerticalAlignment = VerticalAlignment.Stretch,

                            Margin = new Thickness(ToPixel(s * rate - 1), 5, 0, 0)
                        };
                        border.SetResourceReference(BackgroundProperty, "MaterialDesignBody");
                        scale.Children.Add(border);
                        if (s is not 0)
                        {
                            scale.Children.Add(new TextBlock()
                            {
                                Margin = new Thickness(ToPixel(s * rate + 1), 0, 0, 0),
                                HorizontalAlignment = HorizontalAlignment.Left,
                                VerticalAlignment = VerticalAlignment.Top,
                                Text = s.ToString() + " sec"
                            });
                        }

                        //以下はフレーム
                        if (zoom <= 200 && zoom >= 166.7)
                        {
                            for (int m = 1; m < rate; m++)
                            {
                                Border border2 = new Border
                                {
                                    Width = 1,
                                    HorizontalAlignment = HorizontalAlignment.Left,

                                    Margin = new Thickness(ToPixel(s * rate - 1 + m), top, 0, 0)
                                };

                                border2.SetResourceReference(BackgroundProperty, "MaterialDesignBodyLight");
                                scale.Children.Add(border2);
                            }
                        }
                        else if (zoom < 166.7 && zoom >= 133.4)
                        {
                            for (int m = 1; m < rate / 2; m++)
                            {
                                Border border2 = new Border
                                {
                                    Width = 1,
                                    HorizontalAlignment = HorizontalAlignment.Left,

                                    Margin = new Thickness(ToPixel(s * rate - 1 + m * 2), top, 0, 0)
                                };

                                border2.SetResourceReference(BackgroundProperty, "MaterialDesignBodyLight");
                                scale.Children.Add(border2);
                            }
                        }
                        else if (zoom < 133.4 && zoom >= 100)
                        {
                            for (int m = 1; m < rate / 4; m++)
                            {
                                Border border2 = new Border
                                {
                                    Width = 1,
                                    HorizontalAlignment = HorizontalAlignment.Left,

                                    Margin = new Thickness(ToPixel(s * rate - 1 + m * 4), top, 0, 0)
                                };

                                border2.SetResourceReference(BackgroundProperty, "MaterialDesignBodyLight");
                                scale.Children.Add(border2);
                            }
                        }
                    }
                }
                else
                {
                    //m は分数
                    //最大の分
                    for (int m = 1; m < (max / rate) / 60; m++)
                    {
                        Border border = new Border()
                        {
                            Width = 1,
                            HorizontalAlignment = HorizontalAlignment.Left,
                            VerticalAlignment = VerticalAlignment.Stretch,

                            Margin = new Thickness(MinToPixel(m), 5, 0, 0)
                        };
                        border.SetResourceReference(BackgroundProperty, "MaterialDesignBody");
                        scale.Children.Add(border);

                        for (int s = 1; s < 60; s++)
                        {
                            Border border2 = new Border
                            {
                                Width = 1,
                                HorizontalAlignment = HorizontalAlignment.Left,

                                Margin = new Thickness(SecToPixel(s + m / 60), 15, 0, 0)
                            };

                            border2.SetResourceReference(BackgroundProperty, "MaterialDesignBodyLight");
                            scale.Children.Add(border2);
                        }
                    }
                }
            });
        }

        private void ScrollLine_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            Task.Run(() =>
            {
                _Scene.TimeLineHorizonOffset = ScrollLine.HorizontalOffset;
                _Scene.TimeLineVerticalOffset = ScrollLine.VerticalOffset;
            });
        }
    }
}
