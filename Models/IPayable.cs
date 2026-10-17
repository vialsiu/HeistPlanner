namespace HeistPlanner.Models
{
    //---same idea as IEmployee.GetSalary() from class, anyone who gets paid from the loot
    public interface IPayable
    {
        public decimal GetCutPercentage();
    }
}