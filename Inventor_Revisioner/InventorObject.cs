using System;
using System.Windows;
using Inventor;
using File = System.IO.File;
using Path = System.IO.Path;

namespace Inventor_Revisioner
{
    public abstract class InventorObject
    {
        public string DocumentName { get; private set; }
        public string NextRevisionNumber { get; private set; }
        public bool HasRevision { get; private set; }
        public bool HasDrawing { get; private set; }
        private readonly Inventor.Application _inventorObject;
        private string FullPath { get; set; }
        private string FullPathWithRev { get; set; }
        private string DrawingPath { get; set; }
        private string DrawingPathWithRev { get; set; }
        private string PathWithDocument { get; set; }
        private string Directory { get; set; }
        protected virtual string FileExtension { get; set; }
        protected virtual string ObjectType { get; set; }

        // Constructor
        protected InventorObject(Inventor.Application inventorObject)
        {
            this._inventorObject = inventorObject;
        }


        // Update function
        public void UpdateInformation()
        {
            // Check if active document is of type assembly or part
            if (!Utility.DocumentChecker(Utility.DataTypes.Assembly, this._inventorObject) && !Utility.DocumentChecker(Utility.DataTypes.Part, this._inventorObject))
            {
                MessageBox.Show("Kann nur in einer Baugruppe oder Bauteil ausgeführt werden.");
                return;
            }

            // Full path of part/assembly
            this.FullPath = this._inventorObject.ActiveDocument.FullFileName;
            // Directory
            this.Directory = Path.GetDirectoryName(this.FullPath);
            // Document name without type
            this.DocumentName = Path.GetFileNameWithoutExtension(this.FullPath);
            // Document name with path
            this.PathWithDocument = $@"{this.Directory}\{this.DocumentName}";

            // Check if drawing exists
            var drawingPath = this.PathWithDocument + ".idw";
            this.HasDrawing = File.Exists(@drawingPath);

            // Check if file has revision
            this.HasRevision = Utility.RevisionChecker(this.DocumentName);

            // Calculate next revision
            this.NextRevisionNumber = this.HasRevision ? Utility.NummericRevision(this.DocumentName).ToString() : "01";
        }

        // Copy function
        private void NextRevision()
        {
            // Revision Setup
            var prefix = this.HasRevision ? Utility.NummericRevision(this.DocumentName) : "01";
            if (this.HasRevision)
            {
                var delimiter = this.PathWithDocument.LastIndexOf('R') - 1;
                this.PathWithDocument = this.PathWithDocument.Substring(0, delimiter);
            }

            // Copy pre-setup
            this.DrawingPath = $"{this.FullPath.Substring(0, this.FullPath.Length - 4)}.idw";
            this.FullPathWithRev = $"{this.PathWithDocument } Rev.{prefix}{this.FileExtension}";
            this.DrawingPathWithRev = $"{this.PathWithDocument } Rev.{prefix}.idw";

            if (File.Exists(FullPathWithRev) || File.Exists(DrawingPathWithRev))
            {
                var result = MessageBox.Show($"{(this.HasDrawing ? "Zeichnung und " : "")}{this.ObjectType} besteht. \nÜberschreiben?", "Warnung",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.No)
                {
                    return;
                }
            }

            // If drawing applicable
            if (this.HasDrawing)
            {
                try
                {
                    System.IO.File.Copy(this.FullPath, this.FullPathWithRev, true);
                    System.IO.File.Copy(this.DrawingPath, this.DrawingPathWithRev, true);
                    MessageBox.Show($"{this.ObjectType} und Zeichnung erfolgreich revisioniert!");
                }
                catch (Exception error)
                {
                    MessageBox.Show("Da hat etwas nicht funktioniert..." + error);
                    throw;
                }
            }
            else
            {
                // Copy of assembly / part
                try
                {
                    System.IO.File.Copy(this.FullPath, this.FullPathWithRev, true);
                    MessageBox.Show($"{this.ObjectType} erfolgreich revisioniert!");
                }
                catch (Exception error)
                {
                    MessageBox.Show("Da hat etwas nicht funktioniert: " + error.Message);
                    throw;
                }

            }

        }

        private void OpenDrawingAndReplace()
        {
            // Opens the copied drawing (with new revision)
            if (this.HasDrawing)
            {
                // Silent Inventor on
                this._inventorObject.SilentOperation = true;
                // Open drawing
                this._inventorObject.Documents.Open(this.DrawingPathWithRev, true);
                var revisedDrawing = this._inventorObject.ActiveDocument.File;
                // Replace all references
                var allDrawingReferences = revisedDrawing.ReferencedFileDescriptors;
                try
                {
                    foreach (FileDescriptor reference in allDrawingReferences)
                    {
                        if (reference.FullFileName == this.FullPath)
                        {
                            reference.ReplaceReference(this.FullPathWithRev);
                            MessageBox.Show($"{this.ObjectType}referenz ersetzt.");
                        }
                    }
                }
                catch (Exception error)
                {
                    MessageBox.Show("Da hat etwas nicht funktioniert: " + error.Message);
                    throw;
                }
                // Save drawing
                this._inventorObject.ActiveDocument.Save();
                // Close drawing
                this._inventorObject.ActiveDocument.Close();

                // Silent Inventor off
                this._inventorObject.SilentOperation = false;
            }
        }

        public void CopyAndReplace()
        {
            // Copies the assembly / part and associated drawing if applicable
            this.NextRevision();
            // Replace the reference to the assembly / part in the drawing
            this.OpenDrawingAndReplace();
        }

    }
}