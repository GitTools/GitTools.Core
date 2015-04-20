namespace GitTools
{
    using System.Text;

    public static class StringBuilderExtensions
    {
        //[StringFormatMethod("format")]
        public static void AppendLineFormat(this StringBuilder stringBuilder, string format, params object[] args)
        {
            stringBuilder.AppendFormat(format, args);
            stringBuilder.AppendLine();
        }
    }
}