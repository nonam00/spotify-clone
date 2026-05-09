using Domain.Models;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityTypeConfigurations;

public class LyricsSegmentConfiguration : IEntityTypeConfiguration<LyricsSegment>
{
    public void Configure(EntityTypeBuilder<LyricsSegment> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();

        builder.OwnsOne(ls => ls.LyricsSegmentData, lyricsSegmentDataBuilder =>
        {
            lyricsSegmentDataBuilder.Property(p => p.Start)
                .HasColumnName("start")
                .IsRequired();
                
            lyricsSegmentDataBuilder.Property(p => p.End)
                .HasColumnName("end")
                .IsRequired();

            lyricsSegmentDataBuilder.Property(p => p.Text)
                .HasColumnName("text")
                .IsRequired();
                
            lyricsSegmentDataBuilder.Property(p => p.Order)
                .HasColumnName("order")
                .IsRequired();
        });
        
        builder.Property<string>("NormalizedText")
            .HasComputedColumnSql("lower(f_unaccent(trim(\"text\")))", stored: true);
            
        // Gin indexes for trigram searching
        builder.HasIndex("NormalizedText")
            .HasMethod("gin")
            .HasOperators("gin_trgm_ops");
    }
}