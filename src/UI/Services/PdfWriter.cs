using Backend.Models;
using PdfSharp.Drawing.Layout;
using PdfSharp.Drawing;
using PdfSharp.Pdf; 
using System.IO; 
using System.Text;
using System.Globalization;
namespace UI.Services
{
    public class PdfWriter
    {
        private readonly double _leftMargin = XUnit.FromPoint(40).Point; 
        private readonly double _verticalMargin = XUnit.FromPoint(20).Point;
        private readonly double _lineHeight = XUnit.FromPoint(15).Point;

        private readonly XFont fontMuscleGroup = new("Arial", 16);
        private readonly XFont fontTitle = new("Arial", 13);
        private readonly XFont fontStep = new("Arial", 10);

        private readonly XStringFormat _textFormat = XStringFormats.TopLeft;
        private readonly XBrush _fontColor = XBrushes.Black;

        public void Save(string filename, IEnumerable<Exercise> exercises)
        {
            using PdfDocument document = new();
            document.Info.Title = "FlexPoint Exercises PDF";
            var groupedExercises = exercises.GroupBy(e => e.TargetMuscles.FirstOrDefault() ?? "");

            foreach (var group in groupedExercises)
            {
                PdfPage page = document.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);
                XTextFormatter textFormatter = new(gfx);
                double contentWidth = page.Width.Point - 2 * _leftMargin;
                double currentPosition = _verticalMargin;
                currentPosition = PrintHeader(gfx, textFormatter, group.Key, contentWidth, currentPosition);
                currentPosition = PrintExercises(gfx, textFormatter, page, group, contentWidth, currentPosition, document);
            }

            document.Save(filename);
        }

        private double PrintHeader(XGraphics gfx, XTextFormatter textFormatter, string muscleGroup, double contentWidth, double currentPosition)
        {
            string imagePath = $"Assets/{muscleGroup}.png";
            if (File.Exists(imagePath))
            {
                XImage image = XImage.FromFile(imagePath);
                double scaleFactor = 0.05;
                double imageWidth = image.PixelWidth * scaleFactor;
                double imageHeight = image.PixelHeight * scaleFactor;
                gfx.DrawImage(image, _leftMargin, currentPosition, imageWidth, imageHeight);
                currentPosition += imageHeight + 5; 
            }

            var muscleGroupRect = new XRect(_leftMargin, currentPosition, contentWidth, _verticalMargin); 
            textFormatter.DrawString(muscleGroup.ToUpper(), fontMuscleGroup, _fontColor, muscleGroupRect, _textFormat);
            currentPosition += _verticalMargin;
            DrawUnderline(gfx, contentWidth, currentPosition);
            return currentPosition += _verticalMargin / 2;
        }

        private double PrintExercises(XGraphics gfx, XTextFormatter textFormatter, PdfPage page, IGrouping<string, Exercise> group, double contentWidth, double currentPosition, PdfDocument document)
        {
            foreach (var exercise in group)
            {
                double requiredHeight = CalculateRequiredHeight(exercise);
                if (IsNewPageRequired(currentPosition, requiredHeight, page))
                {
                    page = document.AddPage();
                    gfx = XGraphics.FromPdfPage(page);
                    textFormatter = new XTextFormatter(gfx);
                    currentPosition = _verticalMargin;
                }

                var titleRect = new XRect(_leftMargin, currentPosition, contentWidth, _verticalMargin); 
                textFormatter.DrawString(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(exercise.Name), fontTitle, _fontColor, titleRect, _textFormat);
                currentPosition += _verticalMargin + 5;

                var stepsTextBuilder = new StringBuilder();
                foreach (var step in exercise.Instructions)
                {
                    stepsTextBuilder.AppendLine(step);
                }

                string stepsText = stepsTextBuilder.ToString();
                double stepsHeight = exercise.Instructions.Count * _lineHeight;
                var instructionsRect = new XRect(_leftMargin, currentPosition, contentWidth, stepsHeight);
                textFormatter.DrawString(stepsText, fontStep, _fontColor, instructionsRect, _textFormat);
                currentPosition += stepsHeight + _verticalMargin;
            }
            return currentPosition;
        }

        private void DrawUnderline(XGraphics gfx, double contentWidth, double currentPosition)
        {
            gfx.DrawLine(XPens.Black, _leftMargin, currentPosition, _leftMargin + contentWidth, currentPosition);
        }

        private double CalculateRequiredHeight(Exercise exercise)
        {
            double stepsHeight = exercise.Instructions.Count * _lineHeight;
            return _verticalMargin + stepsHeight + _verticalMargin;
        }

        private static bool IsNewPageRequired(double currentPosition, double requiredHeight, PdfPage page)
        {
            double bottomMargin = XUnit.FromPoint(50).Point;
            return currentPosition + requiredHeight > page.Height.Point - bottomMargin;
        }

    }
}
