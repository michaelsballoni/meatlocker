namespace MeetLocker
{
    [TestClass]
    public sealed class MgrTests
    {
        [TestMethod]
        public void TestFilesDirPath()
        {
            string files_dir_path = Mgr.FilesDirPath;
            Assert.IsTrue(Directory.Exists(files_dir_path));

            string files_dir_path2 = Mgr.FilesDirPath;
            Assert.IsTrue(Directory.Exists(files_dir_path2));
        }

        [TestMethod]
        public void TestGetFilePath()
        {
            string file_path_null = Mgr.GetFilePath("", "");
            Assert.AreEqual(file_path_null, Mgr.GetFilePath("", ""));

            string file_path_set = Mgr.GetFilePath("key", "val");
            Assert.AreEqual(file_path_set, Mgr.GetFilePath("key", "val"));

            string file_path_long = Mgr.GetFilePath("keykeykeykeykeykey", "valvalvalvalvalvalval");
            Assert.AreEqual(file_path_long, Mgr.GetFilePath("keykeykeykeykeykey", "valvalvalvalvalvalval"));
        }

        [TestMethod]
        public void StoreRetrieve()
        {
            string test_pwd = "blet monkey";

            string test_file_path = Path.Combine(Path.GetTempPath(), "store-retrieve-test.txt");
            string test_file_contents = "blet monkey";
            File.WriteAllText(test_file_path, test_file_contents);

            string key;
            using (Stream test_file_stream = File.OpenRead(test_file_path))
                key = Mgr.Store(test_pwd, Path.GetFileName(test_file_path), test_file_stream).Result;

            Mgr.Retrieve(key, test_pwd, (filename) => File.OpenWrite(test_file_path + "-" + filename)).Wait();

            Assert.AreEqual(File.ReadAllText(test_file_path), File.ReadAllText(test_file_path + "-" + "store-retrieve-test.txt"));
        }
    }
}
