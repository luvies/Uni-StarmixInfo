namespace StarmixInfo.Models.Data
{
    public class Project
    {
        // primary key
        public int ProjectID { get; set; }

        public string Name { get; set; }
        public string ShortDesc { get; set; }
        public string LongDesc { get; set; }
        public string UnityOrgID { get; set; }
        public string UnityProjectID { get; set; }
        public string GDocFolderID { get; set; }
    }
}
