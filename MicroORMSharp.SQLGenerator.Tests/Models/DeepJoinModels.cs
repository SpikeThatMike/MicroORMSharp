using MicroORMSharp.SqlGenerator.Attributes;
using MicroORMSharp.SqlGenerator.Interfaces;

namespace MicroORMSharp.SqlGenerator.Tests.Models
{
    [DbTable("DeepJoinLevel1")]
    public class DeepJoinLevel1 : IMicroORMSharp
    {
        [DbIdentity]
        public long Id { get; set; }

        [DBJoin(typeof(DeepJoinLevel2), "Id", "Level1Id", DBJoinType.Inner)]
        public DeepJoinLevel2 Level2 { get; set; }
    }

    [DbTable("DeepJoinLevel2")]
    public class DeepJoinLevel2 : IMicroORMSharp
    {
        [DbIdentity]
        public long Id { get; set; }

        public long Level1Id { get; set; }

        [DBJoin(typeof(DeepJoinLevel3), "Id", "Level2Id", DBJoinType.Inner)]
        public DeepJoinLevel3 Level3 { get; set; }
    }

    [DbTable("DeepJoinLevel3")]
    public class DeepJoinLevel3 : IMicroORMSharp
    {
        [DbIdentity]
        public long Id { get; set; }

        public long Level2Id { get; set; }

        [DBJoin(typeof(DeepJoinLevel4), "Id", "Level3Id", DBJoinType.Inner)]
        public DeepJoinLevel4 Level4 { get; set; }
    }

    [DbTable("DeepJoinLevel4")]
    public class DeepJoinLevel4 : IMicroORMSharp
    {
        [DbIdentity]
        public long Id { get; set; }

        public long Level3Id { get; set; }

        [DBJoin(typeof(DeepJoinLevel5), "Id", "Level4Id", DBJoinType.Inner)]
        public DeepJoinLevel5 Level5 { get; set; }
    }

    [DbTable("DeepJoinLevel5")]
    public class DeepJoinLevel5 : IMicroORMSharp
    {
        [DbIdentity]
        public long Id { get; set; }

        public long Level4Id { get; set; }

        [DBJoin(typeof(DeepJoinLevel6), "Id", "Level5Id", DBJoinType.Inner)]
        public DeepJoinLevel6 Level6 { get; set; } 
    }

    [DbTable("DeepJoinLevel6")]
    public class DeepJoinLevel6 : IMicroORMSharp
    {
        [DbIdentity]
        public long Id { get; set; }

        public long Level5Id { get; set; }
    }
}
