using Moq;
using Xunit;
using System.Collections.Generic;
using System.IO;
using Backend.Models;
using PdfSharp.Drawing.Layout;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using UI.Services;

namespace UI.Tests
{
    public class PdfWriterTests
    {
        private readonly List<Exercise> _exercises =
    [
        new() {
            ExerciseId = "1",
            Name = "Push Up",
            TargetMuscles = ["Chest"],
            Instructions = ["Get into a plank position", "Lower your body and dont die"]
        },
        new() {
            ExerciseId = "2",
            Name = "Bicep Curl",
            TargetMuscles = ["Biceps"],
            Instructions = ["Stand with dumbbells", "Curl the weights up and dont cry"]
        }
    ];


        [Fact]
        public void SaveAndDeleteSimple()
        {
            var pdfWriter = new PdfWriter();
            string outputPath = "output.pdf";
            try
            {
                pdfWriter.Save(outputPath, _exercises);
                Assert.True(File.Exists(outputPath), "PDF not created.");
                var fileInfo = new FileInfo(outputPath);
                Assert.True(fileInfo.Length > 0, "The PDF is empty.");
            }
            finally
            {
                if (File.Exists(outputPath))
                {
                    File.Delete(outputPath);
                }
            }
        }

    }
}