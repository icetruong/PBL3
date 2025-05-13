namespace HoldEvent.Models
{
    public class RevenueReportAdminViewModel
    {
        public string SourceName { get; set; }          
        public string Type { get; set; }                 
        public decimal CommissionPercent { get; set; }   
        public decimal TotalRevenue { get; set; }        
        public decimal CommissionEarned { get; set; }
    }
}
