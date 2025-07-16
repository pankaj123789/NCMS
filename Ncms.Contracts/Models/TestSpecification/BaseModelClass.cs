
namespace Ncms.Contracts.Models.TestSpecification
{
    public class BaseModelClass
    {
        public BaseModelClass()
        {
            ModelState = ModelState.unaltered;
        }

        public ModelState ModelState { get; set; }
        public string NaturalKey { get; set; }
    }

    public enum ModelState
    {
        unaltered,
        altered,
        deleted,
        inserted
    }

}
