using System.Text;

namespace ZxXpFly_WiFI
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // ¡ï¡ï¡ï ×îÏÈ×¢²á ¡ï¡ï¡ï
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new passwordName());
        }
    }
}