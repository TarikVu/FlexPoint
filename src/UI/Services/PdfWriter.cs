using Backend.Models;
using PdfSharp.Drawing.Layout;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

            XFont fontMuscleGroup = new("Verdana", 14); // Font for muscle category
            XFont fontTitle = new("Verdana", 12); // Font for exercise name
            XFont fontStep = new("Verdana", 10); // Font for steps
            double contentWidth = page.Width.Point - 2 * _leftMargin;
            double currentYPosition = _topMargin;

            var groupedExercises = exercises.GroupBy(e => e.TargetMuscles.FirstOrDefault() ?? "Unknown");

            foreach (var group in groupedExercises)
            {
                string imagePath = $"Assets/{group.Key.ToLower()}.png";
                double imageWidth = 0, imageHeight = 0;

                if (File.Exists(imagePath))
                {
                    XImage image = XImage.FromFile(imagePath);
                    double scaleFactor = 0.05;
                    imageWidth = image.PixelWidth * scaleFactor;
                    imageHeight = image.PixelHeight * scaleFactor;

                    gfx.DrawImage(image, _leftMargin, currentYPosition, imageWidth, imageHeight);

                    var muscleGroupRect = new XRect(_leftMargin + imageWidth + 10, currentYPosition + imageHeight - _lineHeightTitle, contentWidth - imageWidth - 10, _lineHeightTitle);
                    textFormatter.Alignment = XParagraphAlignment.Left;
                    textFormatter.DrawString(group.Key.ToUpper(), fontMuscleGroup, XBrushes.Black, muscleGroupRect, XStringFormats.TopLeft);

                    currentYPosition += imageHeight + _extraSpaceBetweenExercises;
                }
                else
                {
                    var muscleGroupRect = new XRect(_leftMargin, currentYPosition, contentWidth, _lineHeightTitle);
                    textFormatter.Alignment = XParagraphAlignment.Left;
                    textFormatter.DrawString(group.Key.ToUpper(), fontMuscleGroup, XBrushes.Black, muscleGroupRect, XStringFormats.TopLeft);
                    currentYPosition += _lineHeightTitle + _extraSpaceBetweenExercises;
                }

                gfx.DrawLine(XPens.Black, _leftMargin, currentYPosition, page.Width.Point - _leftMargin, currentYPosition);
                currentYPosition += _extraSpaceBetweenExercises / 2;

                foreach (var exercise in group)
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
