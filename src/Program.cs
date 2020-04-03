using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;

namespace AllocTest
{
    public class Bench
    {
        private static void Main(string[] args) {
            var iter = 0;
            Console.WriteLine($"PID: {Process.GetCurrentProcess().Id}");
            while (Console.ReadLine() != "e") {
                var preAlloc = GetGcInfo();

                Console.WriteLine($"Iteration: {iter++}");
                Console.WriteLine("Before cv.Mat alloc");
                Console.ReadLine();
                var matrices = new List<Mat>();
                for (var i = 0; i < 1000; i++)
                {
                    var mat = new Mat(100, 8000, MatType.CV_8U, Scalar.RoyalBlue);
                    mat.Set(5, (byte)42);
                    matrices.Add(mat);
                }
                var postAlloc = GetGcInfo();

                Console.WriteLine("After cv.Mat alloc");
                Console.ReadLine();

                foreach (var mat in matrices)
                {
                    mat.Dispose();
                }
                matrices.Clear();
                matrices = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
                var postGc = GetGcInfo();


                Console.WriteLine("After dispose and forced GC");
                Console.WriteLine($"Pre: \n{preAlloc}\n\n" +
                                  $"After alloc \n{postAlloc}\n\n" +
                                  $"Post GC:\n{postGc}");
            }

            string GetGcInfo() {
                return JsonSerializer.Serialize(GC.GetGCMemoryInfo(),
                    new JsonSerializerOptions {WriteIndented = true});
            }
        }
    }
}