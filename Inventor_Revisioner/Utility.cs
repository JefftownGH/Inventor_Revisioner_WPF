using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Inventor;

namespace Inventor_Revisioner
{
    public static class Utility
    {
        public enum DataTypes
        {
            Assembly = 0,
            Drawing = 1,
            Part = 2
        }
        // Function for checking if the active document is of given type
        public static bool DocumentChecker(DataTypes allowedDocument, Inventor.Application inventorObject)
        {
            bool isValid;
            switch (allowedDocument)
            {
                case DataTypes.Assembly:
                    isValid = inventorObject.ActiveDocumentType == DocumentTypeEnum.kAssemblyDocumentObject ? true : false;
                    break;
                case DataTypes.Drawing:
                    isValid = inventorObject.ActiveDocumentType == DocumentTypeEnum.kDrawingDocumentObject ? true : false;
                    break;
                case DataTypes.Part:
                    isValid = inventorObject.ActiveDocumentType == DocumentTypeEnum.kPartDocumentObject ? true : false;
                    break;
                default:
                    isValid = false;
                    break;
            }

            return isValid;
        }

        // Function for checking if the assembly already has a revision
        public static bool RevisionChecker(string documentName)
        {
            var rgx = new Regex(@"(?i)rev");
            return rgx.IsMatch(documentName);
        }

        // Function for returning the next revision
        public static string NummericRevision(string documentName)
        {
            var delimiter = documentName.LastIndexOf('.');
            var currentRevision = int.Parse(documentName.Substring(delimiter + 1));
            var nextRevision = (++currentRevision).ToString();
            return nextRevision.Length == 1 ? $"0{nextRevision}" : nextRevision;
        }

        // Function for returning the previous revision
        public static string PrevNummericRevision(string documentName)
        {
            var delimiter = documentName.LastIndexOf('.');
            var currentRevision = int.Parse(documentName.Substring(delimiter + 1));
            var prevRevision = (--currentRevision).ToString();
            return prevRevision.Length == 1 ? $"0{prevRevision}" : prevRevision;
        }

        // Function for validating the current active document with the instance document
        public static bool CurrentDocument(Inventor.Application currentDocument, Inventor.Application instanceDocument)
        {
            return currentDocument == instanceDocument;
        }
    }
}