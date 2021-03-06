﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEditor
{
    /// <summary>
    /// Represents a service in a file dialog.
    /// </summary>
    public interface IFileDialogService
    {
        /// <summary>
        /// Show the Save File dialog.
        /// </summary>
        public bool ShowSaveFileDialog(SaveFileRecord record);
        /// <summary>
        /// Show the Open File dialog.
        /// </summary>
        public bool ShowOpenFileDialog(OpenFileRecord record);
    }

    /// <summary>
    /// Represents the record to be used when showing the Save File dialog.
    /// </summary>
    public record SaveFileRecord : FileDialogRecord
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SaveFileRecord"/> class.
        /// </summary>
        public SaveFileRecord(string filename = "") : base(new List<FileFilter>())
        {
            FileName = filename;
        }

        /// <summary>
        /// Gets or sets the default file name.
        /// </summary>
        public string DefaultFileName { get; set; } = "";
        /// <summary>
        /// Gets or sets the file name.
        /// </summary>
        public string FileName { get; set; }
    }
    /// <summary>
    /// Represents the record to be used when showing the Open File dialog.
    /// </summary>
    public record OpenFileRecord : FileDialogRecord
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenFileRecord"/> class.
        /// </summary>
        public OpenFileRecord() : base(new List<FileFilter>())
        {

        }

        /// <summary>
        /// Gets or sets the default file name.
        /// </summary>
        public string DefaultFileName { get; set; } = "";
        /// <summary>
        /// Gets or sets the file name.
        /// </summary>
        public string FileName { get; set; } = "";
    }
    /// <summary>
    /// Represents the extension of the file.
    /// </summary>
    public record FileExtension
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileExtension"/> class.
        /// </summary>
        /// <param name="Value">The file extension.</param>
        public FileExtension(string Value)
        {
            this.Value = Value;
        }

        /// <summary>
        /// Get the file extension
        /// </summary>
        public string Value { get; init; }
    }
    /// <summary>
    /// Represents the filter of the file dialog.
    /// </summary>
    public record FileFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileFilter"/> class.
        /// </summary>
        /// <param name="Name">The filter name.</param>
        /// <param name="Extensions">The filter extensions.</param>
        public FileFilter(string Name, IEnumerable<FileExtension> Extensions)
        {
            this.Name = Name;
            this.Extensions = Extensions;
        }

        /// <summary>
        /// Get the filter name.
        /// </summary>
        public string Name { get; init; }
        /// <summary>
        /// Get the filter extensions.
        /// </summary>
        public IEnumerable<FileExtension> Extensions { get; init; }
    }
    /// <summary>
    /// Represents the base record to be used when showing the File dialog.
    /// </summary>
    public record FileDialogRecord
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileDialogRecord"/> class.
        /// </summary>
        /// <param name="Filters">The filter for the file dialog.</param>
        public FileDialogRecord(List<FileFilter> Filters)
        {
            this.Filters = Filters;
        }

        /// <summary>
        /// Get the filter for the file dialog.
        /// </summary>
        public List<FileFilter> Filters { get; init; }
    }
}
