using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using dCForm.Core.Storage.Sql;

namespace dCForm.Core
{
    //TODO:Move methods to a Sql.BaseDocExtensions file

    /// <summary>
    ///     The base of all entities utilizing a single auto increment integer id.
    ///     Concurrency issues EntityFramework 6.x CodeFirst method presents require
    ///     the logic here to ensure Proxied-Tracked objects make it in &
    ///     out of the database & memory correctly.
    ///     It is Serializable for JavaScriptSerialization
    /// </summary>
    [DataContract(Namespace = "http://schemas.progablab.com/2014/12/Serialization")]
    [Serializable]
    public abstract class BaseAutoIdent
    {
        [Key]
        [IgnoreDataMember]
        [XmlIgnore]
        [ScriptIgnore]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id
        {
            get;
            set;
        }

        private DbSet set(SqlDB db)
        {
            return db.UnderlyingDbContext.Set(entityType);
        }

        private DbEntityEntry entry(SqlDB db)
        {
            return db.UnderlyingDbContext.Entry(this);
        }

        private Type entityType
        {
            get
            {
                return ObjectContext.GetObjectType(GetType());
            }
        }

        private string pkStr(SqlDB db)
        {
            return Id.ToString();
        }

        private BaseAutoIdent attachedEntity(SqlDB db)
        {
            return set(db)
                .Local
                .AsQueryable()
                .Cast<BaseAutoIdent>()
                .FirstOrDefault(m => m.pkStr(db) == pkStr(db));
        }

        private void Update(SqlDB db)
        {
            if (entry(db).State == EntityState.Detached)
                if (attachedEntity(db) != null)
                    db.UnderlyingDbContext.Entry(attachedEntity(db)).CurrentValues.SetValues(this);
                else
                    entry(db).State = EntityState.Modified; // This should attach entity
        }

        private void Add(SqlDB db)
        {
            set(db).Add(this);
        }

        /// <summary>
        ///     attaches the objects graph & it's child objects via navigation properties back the the DBContext & executes
        ///     Adds/Updates. This object graph attacher works specifically with BaseAutoIdent & generic lists of them
        /// </summary>
        /// <param name="db"></param>
        /// <param name="AutoSaveChanges"></param>
        public void Save(SqlDB db, bool AutoSaveChanges = true)
        {
            if (Id == 0)
                Add(db);
            else
                Update(db);

            foreach (PropertyInfo _PropertyInfo in GetType().GetProperties())
                if (_PropertyInfo.GetValue(this, null) != null)
                    if (_PropertyInfo.PropertyType.IsSubclassOf(typeof (BaseAutoIdent)))
                        ((BaseAutoIdent) _PropertyInfo.GetValue(this, null)).Save(db, false);
                    else if (_PropertyInfo.PropertyType.GetInterface("IList") != null)
                        foreach (BaseAutoIdent _BaseAutoIdent in ((IList) _PropertyInfo.GetValue(this, null)).OfType<BaseAutoIdent>())
                            _BaseAutoIdent.Save(db, false);


            if (AutoSaveChanges)
                db.UnderlyingDbContext.SaveChanges();
        }
    }
}