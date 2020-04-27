using DevExpress.Mvvm.POCO;

namespace GTI.WFMS.Models.Mntc.Model
{
    public class ResSrc
    {
        public static ResSrc Create()
        {
            return ViewModelSource.Create(() => new ResSrc());
        }
        public static ResSrc Create(int Id, string Name)
        {
            ResSrc resSrc = ResSrc.Create();
            resSrc.Id = Id;
            resSrc.Name = Name;
            return resSrc;
        }

        protected ResSrc() { }

        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
    }
}
