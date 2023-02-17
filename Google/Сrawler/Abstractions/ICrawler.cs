namespace Google.Сrawler.Abstractions;

public interface ICrawler
{
    public void Scan(Queue<string?> urls);

    public void SetResultsDir(string baseDir = "");
}