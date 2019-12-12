namespace idetector.Models
{
    public class CheckedMessage
    {
        public string Message;
        public bool isTrue;
        public string Classname;


        /// <summary>
        /// Constructor for checks that need a message to be returned
        /// </summary>
        /// <param name="message">Human Readable message</param>
        /// <param name="istrue">Whether or not the check has passed</param>
        /// <param name="classname">Classname of the class used in the check</param>

        public CheckedMessage(string message, bool istrue, string classname)
        {
            Message = message;
            isTrue = istrue;
            Classname = classname;
        }
        /// <summary>
        /// Constructor for checks that do not need a message
        /// </summary>
        /// <param name="istrue">Whether or not the check has passed</param>
        public CheckedMessage(bool istrue)
        {
            isTrue = istrue;
        }
    }
}