using ExpressiveFit.Models.Activities;
using ExpressiveFit.Readers;

namespace ExpressiveFit.Utils;

public static class ReadUtils
{
    public static async Task<List<Activity>> ReadDirectory(string directory)
    {
        var filePaths = Directory.GetFiles(directory).Where(f => f.ToLower().EndsWith(".fit")).ToList();
        return await ReadFiles(filePaths);
    }
    public static async Task<List<Activity>> ReadFiles(List<string> filePaths)
    {
        var files = filePaths.Select(f => new FileStream(f, FileMode.Open)).ToList(); ;
        return await ReadFiles(files);
    }
    public static async Task<List<Activity>> ReadFiles(List<FileStream> files)
    {
        var activities = new List<Activity>();
        var tasks = new List<Task>();

        void action(FileStream fileStream)
        {
            var activity = ReadFile(fileStream);
            activities.Add(activity);
        }

        foreach (var file in files)
        {
            var task = new Task(() =>
            {
                action(file);
            });

            tasks.Add(task);
            task.Start();
        }

        await Task.WhenAll(tasks);
        return activities;
    }

    public static Activity ReadFile(FileStream file)
    {
        var reader = new FitReader();
        return reader.ReadFitFile(file);
    }

    public static Activity ReadFile(string filePath)
    {
        var stream = new FileStream(filePath, FileMode.Open);
        return ReadFile(stream);
    }
}
