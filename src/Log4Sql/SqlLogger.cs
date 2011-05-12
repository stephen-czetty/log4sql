using System;
using System.Data.SqlTypes;
using System.IO;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using Microsoft.SqlServer.Server;

namespace Log4Sql
{
    [Serializable]
    [SqlUserDefinedType(Format.UserDefined, IsByteOrdered=false, Name="Logger")]
    [CLSCompliant(false)]
    public class SqlLogger : INullable, IBinarySerialize
    {
        [NonSerialized]
        private readonly ILog _log;

        [NonSerialized]
        private static readonly SqlLogger NullInstance = new SqlLogger { _isNull = true };

        public SqlLogger()
        {
            var h = LogManager.GetRepository() as Hierarchy;
            if (h == null)
                throw new SqlTypeException();  // Should never happen, unless log4net changes.

            h.Root.RemoveAllAppenders();
            var pl = new PatternLayout
                         {
                             ConversionPattern = "%d [%2t] %-5p   %m%n"
                         };
            pl.ActivateOptions();

            var fileAppender = new RollingFileAppender
                                   {
                                       AppendToFile = true,
                                       LockingModel = new FileAppender.MinimalLock(),
                                       File = "test.log",
                                       Layout = pl
                                   };
            fileAppender.ActivateOptions();

            BasicConfigurator.Configure(fileAppender);

            _log = LogManager.GetLogger("SqlServerLogger");
        }

        public static SqlLogger Null
        {
            get { return NullInstance; }
        }

        private bool _isNull;

        /// <summary>
        /// Indicates whether a structure is null. This property is read-only.
        /// </summary>
        /// <returns>
        /// <see cref="T:System.Data.SqlTypes.SqlBoolean"/>true if the value of this object is null. Otherwise, false.
        /// </returns>
        public bool IsNull
        {
            get { return _isNull; }
        }

        /// <summary>
        /// Generates a user-defined type (UDT) or user-defined aggregate from its binary form.
        /// </summary>
        /// <param name="r">The <see cref="T:System.IO.BinaryReader"/> stream from which the object is deserialized.
        ///                 </param>
        public void Read(BinaryReader r)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Converts a user-defined type (UDT) or user-defined aggregate into its binary format so that it may be persisted.
        /// </summary>
        /// <param name="w">The <see cref="T:System.IO.BinaryWriter"/> stream to which the UDT or user-defined aggregate is serialized.
        ///                 </param>
        public void Write(BinaryWriter w)
        {
            throw new NotImplementedException();
        }
    }
}