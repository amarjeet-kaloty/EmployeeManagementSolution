namespace EmployeeManagementProject.Domain_Layer.Entities
{
    public abstract class Entity
    {
        public int Id { get; set; }

        protected Entity()
        { }

        protected Entity(int id)
        {
            Id = id;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = obj as Entity;

            return other != null && Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}