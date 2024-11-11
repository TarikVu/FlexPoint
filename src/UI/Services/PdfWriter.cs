using Backend.Models;
using PdfSharp.Drawing.Layout;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace UI.Services
{
    public class PdfWriter
    {
        private readonly double _leftMargin = XUnit.FromPoint(40).Point;
        private readonly double _topMargin = XUnit.FromPoint(50).Point;
        private readonly double _lineHeightTitle = XUnit.FromPoint(20).Point;
        private readonly double _extraSpaceBetweenExercises = XUnit.FromPoint(25).Point;

        public void Save(string filename, IEnumerable<Exercise> exercises)
        {
            using PdfDocument document = new();
            document.Info.Title = "FlexPoint PDF";
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XTextFormatter textFormatter = new(gfx);
             
            XFont fontTitle = new("Verdana", 12);
            XFont fontStep = new("Verdana", 10);
            double contentWidth = page.Width.Point - 2 * _leftMargin;
            double currentYPosition = _topMargin;

            foreach (var exercise in exercises)
            {
                double requiredHeight = CalculateRequiredHeight(exercise);
                if (IsNewPageRequired(currentYPosition, requiredHeight, page))
                {
                    page = document.AddPage();
                    gfx = XGraphics.FromPdfPage(page);
                    textFormatter = new XTextFormatter(gfx);
                    currentYPosition = _topMargin;
                }
                currentYPosition = PrintTitleAndSteps(gfx, textFormatter, exercise, fontTitle, fontStep, contentWidth, currentYPosition);
            }

            document.Save(filename);
        }

        private double CalculateRequiredHeight(Exercise exercise)
        {
            double stepsHeight = exercise.Instructions.Count * XUnit.FromPoint(15).Point;
            return _lineHeightTitle + stepsHeight + _extraSpaceBetweenExercises;
        }

        private static bool IsNewPageRequired(double currentYPosition, double requiredHeight, PdfPage page)
        {
            double bottomMargin = XUnit.FromPoint(50).Point;
            return currentYPosition + requiredHeight > page.Height.Point - bottomMargin;
        }

        private double PrintTitleAndSteps(XGraphics gfx, XTextFormatter textFormatter, Exercise exercise, XFont fontTitle, XFont fontStep, double contentWidth, double currentYPosition)
        {
            var titleRect = new XRect(_leftMargin, currentYPosition, contentWidth, _lineHeightTitle);
            textFormatter.Alignment = XParagraphAlignment.Left;
            textFormatter.DrawString(exercise.Name, fontTitle, XBrushes.Black, titleRect, XStringFormats.TopLeft);
            currentYPosition += _lineHeightTitle + 5;

            string stepsText = string.Join("\n", exercise.Instructions.Select(step => $"- {step}"));
            double stepsHeight = exercise.Instructions.Count * XUnit.FromPoint(15).Point;
            var instructionsRect = new XRect(_leftMargin, currentYPosition, contentWidth, stepsHeight);
            textFormatter.DrawString(stepsText, fontStep, XBrushes.Black, instructionsRect, XStringFormats.TopLeft);

            return currentYPosition + stepsHeight + _extraSpaceBetweenExercises;
        }
    }
}

