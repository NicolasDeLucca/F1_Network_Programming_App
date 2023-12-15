namespace Protocol
{
    public enum FrameHeader { Res = 0, Req = 1 }

    public class Frame
    {
        public string Data {get; set;}
        public Command Command {get; private set;}
        public FrameHeader Header { get; set; }

        public Frame(Command command)
        {
            Command = command;
            Data = "";
        }

        public string[] GetDataParams()
        {
            if (Data != null)
                return BuildDataHelper.GetDataParams(Data);
            
            return null;
        }

        public bool IsPhotoRequest()
        {
            return Command == Command.AddPhotoToReplacement && Header == FrameHeader.Req;
        }

        public bool IsPhotoResponse()
        {
            return Command == Command.DownloadReplacementPhoto && Header == FrameHeader.Res;
        }

        public string ShowResponse()
        {
            if (IsPhotoRequest() || IsPhotoResponse())
                return BuildDataHelper.GetDataParams(Data)[0];

            return Data;
        }
    }
}
