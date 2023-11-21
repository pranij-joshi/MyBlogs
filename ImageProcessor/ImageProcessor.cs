using MyBlogs.DTOModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MyBlogs.ImageProcessor;

public class ImageProcessor
{
    private readonly IFormFile _image;
    
    public ImageProcessor(IFormFile image)
    {
        _image = image;
    }
    public async Task<string> ImageSaving(IFormFile image)
    {
        var uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;
        System.Console.WriteLine(uniqueFileName);


        // Get the path to the folder where you want to store the file
        // var path = AppDomain.CurrentDomain.BaseDirectory;
        var uploadsFolder = Path.Combine("D:\\AppboxTech Internship\\playground\\MyBlogs\\Uploads", "ProfilePics");
        System.Console.WriteLine(uploadsFolder);

        // Ensure the folder exists; create it if not
        // Directory.CreateDirectory(uploadsFolder);

        // Combine the folder path and the unique file name to get the full file path
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await image.CopyToAsync(fileStream);
        }

        return filePath;
    }
}
