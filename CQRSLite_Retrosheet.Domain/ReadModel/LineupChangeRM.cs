namespace CQRSLite_Retrosheet.Domain.ReadModel
{
    public class LineupChangeRM
    {
        public string RetrosheetGameId { get; set; }
        public short EventNumber { get; set; }
        public short Sequence { get; set; }
        public bool IsStarter { get; set; }
        public string PlayerId { get; set; }
        public string Name { get; set; }
        public byte Team { get; set; }
        public byte BattingOrder { get; set; }
        public byte FieldPosition { get; set; }
    }
}
