using Microsoft.Win32;


namespace DeleteMetaFiles
{
    internal class Program
    {
#pragma warning disable CA1416
        static void Main(string[] args)
        {
            //判断是否注册表已经存在项
            try
            {
                Microsoft.Win32.RegistryKey registryRoot = Microsoft.Win32.Registry.CurrentUser;
                Console.WriteLine(registryRoot);

                string[] path = new string[] { "SOFTWARE", "Classes", "Directory", "background", "shell" };
                foreach (var item in path)
                {
                    if (registryRoot != null)
                    {
                        registryRoot = registryRoot.OpenSubKey(item,true);
                    }
                }
                if (registryRoot.OpenSubKey("Deletemeta", true) == null)
                {
                    //存在且未注册时，添加注册表
                    registryRoot = registryRoot.CreateSubKey("Deletemeta", true);
                }
                else
                {
                    registryRoot = registryRoot.OpenSubKey("Deletemeta", true);
                }
                if (registryRoot.OpenSubKey("command", true) == null)
                {
                    RegistryKey cmdKey = registryRoot.CreateSubKey("command", true);
                    //获取当前运行程序的名称
                    cmdKey.SetValue("","\"" + System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName + "\"");
                }
                
            }
            catch (Exception)
            {
                throw;
            }

            //TODO：判断当前文件夹中是否存在与运行程序相同名字的exe文件

            string dirPath = System.Environment.CurrentDirectory.ToString();
            DeletaMetaExtension(dirPath);
        }

        //Delete All .meta Extension in CurrentDirectory;
        public static void DeletaMetaExtension(string dirPath)
        {
            Console.WriteLine("Success");
            //Get All .meta Files
            string[] allFIles = Directory.GetFiles(dirPath, "*.meta", SearchOption.AllDirectories);
            foreach (var item in allFIles)
            {
                //Delete All .meta Files
                if(File.Exists(item))
                {
                    File.SetAttributes(item, FileAttributes.Normal);
                    File.Delete(item);
                }
            }
        }
    }
}

