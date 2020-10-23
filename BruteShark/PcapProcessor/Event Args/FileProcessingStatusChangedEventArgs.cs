
public enum FileProcessingStatus
{
    Started,
    Finished,
    Faild
}

namespace PcapProcessor
{
    public class FileProcessingStatusChangedEventArgs
    {
        public FileProcessingStatus Status { get; set; }
        public string FilePath { get; set; }
    }
}
