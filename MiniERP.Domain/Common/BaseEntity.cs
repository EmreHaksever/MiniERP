using System;

namespace MiniERP.Domain.Common
{
    public abstract class BaseEntity
    {
        // Kurumsal projelerde genellikle int yerine Guid tercih edilir, 
        // verilerin taşınması ve güvenliği için daha iyidir.
        public Guid Id { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        // Soft Delete (Veriyi gerçekten silmeyip silindi olarak işaretleme) için
        public bool IsDeleted { get; set; } = false;
    }
}