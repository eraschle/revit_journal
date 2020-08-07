using System.IO;

namespace DataSource.Helper
{
    public static class Constant
    {
        public const char UnderlineChar = '_';
        public readonly static string Underline = UnderlineChar.ToString();
        
        public const char MinusChar = '-';
        public readonly static string Minus = MinusChar.ToString();

        public const char SpaceChar = ' ';
        public readonly static string Space = SpaceChar.ToString();
        
        public const string Star = "*";

        public const char PointChar = '.';
        public readonly static string Point = PointChar.ToString();

        public const char SlashChar = '/';

        public const char BackSlashChar = '\\';
        public readonly static string BackSlash = BackSlashChar.ToString();

        public static string FolderSeperator = Path.DirectorySeparatorChar.ToString();

        public static char Tabulator = '\t';
    }
}
