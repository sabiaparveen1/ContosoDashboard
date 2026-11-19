using System;
using ContosoDashboard.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ContosoDashboard.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ContosoDashboard.Models.Announcement", b =>
            {
                b.Property<int>("AnnouncementId")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AnnouncementId"));

                b.Property<int>("CreatedByUserId")
                    .HasColumnType("int");

                b.Property<string>("Content")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.Property<DateTime>("ExpiryDate")
                    .HasColumnType("datetime2");

                b.Property<bool>("IsActive")
                    .HasColumnType("bit");

                b.Property<DateTime>("PublishDate")
                    .HasColumnType("datetime2");

                b.Property<string>("Title")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.HasKey("AnnouncementId");

                b.HasIndex("CreatedByUserId");

                b.ToTable("Announcements");

                b.HasData(
                    new
                    {
                        AnnouncementId = 1,
                        CreatedByUserId = 1,
                        Content = "Welcome to the new ContosoDashboard application. This platform will help you manage your tasks and projects more efficiently.",
                        ExpiryDate = new DateTime(2026, 12, 19, 10, 24, 31, 386, DateTimeKind.Utc).AddTicks(0),
                        IsActive = true,
                        PublishDate = new DateTime(2026, 11, 19, 10, 24, 31, 386, DateTimeKind.Utc).AddTicks(0),
                        Title = "Welcome to ContosoDashboard"
                    });
            });

            modelBuilder.Entity("ContosoDashboard.Models.Document", b =>
            {
                b.Property<int>("DocumentId")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("DocumentId"));

                b.Property<DateTime?>("DeletedDate")
                    .HasColumnType("datetime2");

                b.Property<string>("Description")
                    .HasColumnType("nvarchar(max)");

                b.Property<string>("FileExtension")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.Property<string>("FileName")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.Property<long>("FileSizeBytes")
                    .HasColumnType("bigint");

                b.Property<string>("Category")
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnType("nvarchar(50)");

                b.Property<string>("FileStoragePath")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.Property<bool>("IsDeleted")
                    .HasColumnType("bit");

                b.Property<string>("MimeType")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.Property<int?>("ProjectId")
                    .HasColumnType("int");

                b.Property<byte[]>("RowVersion")
                    .IsConcurrencyToken()
                    .ValueGeneratedOnAddOrUpdate()
                    .HasColumnType("rowversion");

                b.Property<string>("Title")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.Property<DateTime>("UpdatedDate")
                    .HasColumnType("datetime2");

                b.Property<DateTime>("UploadedDate")
                    .HasColumnType("datetime2");

                b.Property<int>("UploadedByUserId")
                    .HasColumnType("int");

                b.HasKey("DocumentId");

                b.HasIndex("ProjectId");

                b.HasIndex("UploadedByUserId");

                b.HasIndex("UploadedDate");

                b.HasIndex("IsDeleted", "UploadedDate");

                b.ToTable("Documents");
            });

            modelBuilder.Entity("ContosoDashboard.Models.DocumentActivity", b =>
            {
                b.Property<int>("DocumentActivityId")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("DocumentActivityId"));

                b.Property<string>("Action")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.Property<int?>("DocumentId")
                    .HasColumnType("int");

                b.Property<string>("Details")
                    .HasColumnType("nvarchar(max)");

                b.Property<DateTime>("PerformedDate")
                    .HasColumnType("datetime2");

                b.Property<int>("PerformedByUserId")
                    .HasColumnType("int");

                b.HasKey("DocumentActivityId");

                b.HasIndex("PerformedByUserId");

                b.HasIndex("DocumentId", "PerformedDate");

                b.ToTable("DocumentActivities");
            });

            modelBuilder.Entity("ContosoDashboard.Models.DocumentShare", b =>
            {
                b.Property<int>("DocumentShareId")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("DocumentShareId"));

                b.Property<int>("DocumentId")
                    .HasColumnType("int");

                b.Property<bool>("IsActive")
                    .HasColumnType("bit");

                b.Property<byte[]>("RowVersion")
                    .IsConcurrencyToken()
                    .ValueGeneratedOnAddOrUpdate()
                    .HasColumnType("rowversion");

                b.Property<DateTime?>("RevokedDate")
                    .HasColumnType("datetime2");

                b.Property<int>("SharedByUserId")
                    .HasColumnType("int");

                b.Property<DateTime>("SharedDate")
                    .HasColumnType("datetime2");

                b.Property<int>("SharedWithUserId")
                    .HasColumnType("int");

                b.HasKey("DocumentShareId");

                b.HasIndex("SharedByUserId");

                b.HasIndex("DocumentId", "IsActive");

                b.HasIndex("SharedWithUserId", "IsActive");

                b.ToTable("DocumentShares");
            });

            modelBuilder.Entity("ContosoDashboard.Models.DocumentTag", b =>
            {
                b.Property<int>("DocumentTagId")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("DocumentTagId"));

                b.Property<int>("DocumentId")
                    .HasColumnType("int");

                b.Property<int>("TagId")
                    .HasColumnType("int");

                b.HasKey("DocumentTagId");

                b.HasIndex("TagId");

                b.HasIndex("DocumentId");

                b.ToTable("DocumentTags");
            });

            modelBuilder.Entity("ContosoDashboard.Models.Notification", b =>
            {
                b.Property<int>("NotificationId")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("NotificationId"));

                b.Property<string>("Content")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.Property<DateTime>("CreatedDate")
                    .HasColumnType("datetime2");

                b.Property<bool>("IsRead")
                    .HasColumnType("bit");

                b.Property<string>("NotificationType")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.Property<int>("UserId")
                    .HasColumnType("int");

                b.HasKey("NotificationId");

                b.HasIndex("UserId", "IsRead");

                b.ToTable("Notifications");
            });

            modelBuilder.Entity("ContosoDashboard.Models.Project", b =>
            {
                b.Property<int>("ProjectId")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProjectId"));

                b.Property<DateTime>("CreatedDate")
                    .HasColumnType("datetime2");

                b.Property<string>("Description")
                    .HasColumnType("nvarchar(max)");

                b.Property<string>("Name")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.Property<int>("ProjectManagerId")
                    .HasColumnType("int");

                b.Property<DateTime>("StartDate")
                    .HasColumnType("datetime2");

                b.Property<int>("Status")
                    .HasColumnType("int");

                b.Property<DateTime>("TargetCompletionDate")
                    .HasColumnType("datetime2");

                b.Property<DateTime>("UpdatedDate")
                    .HasColumnType("datetime2");

                b.HasKey("ProjectId");

                b.HasIndex("ProjectManagerId");

                b.HasIndex("Status");

                b.ToTable("Projects");

                b.HasData(
                    new
                    {
                        ProjectId = 1,
                        CreatedDate = new DateTime(2026, 10, 20, 10, 24, 31, 386, DateTimeKind.Utc).AddTicks(0),
                        Description = "Internal employee productivity dashboard",
                        Name = "ContosoDashboard Development",
                        ProjectManagerId = 2,
                        StartDate = new DateTime(2026, 10, 20, 10, 24, 31, 386, DateTimeKind.Utc).AddTicks(0),
                        Status = 0,
                        TargetCompletionDate = new DateTime(2027, 1, 18, 10, 24, 31, 386, DateTimeKind.Utc),
                        UpdatedDate = new DateTime(2026, 11, 19, 10, 24, 31, 386, DateTimeKind.Utc).AddTicks(0)
                    });
            });

            modelBuilder.Entity("ContosoDashboard.Models.ProjectMember", b =>
            {
                b.Property<int>("ProjectMemberId")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProjectMemberId"));

                b.Property<DateTime>("AssignedDate")
                    .HasColumnType("datetime2");

                b.Property<int>("ProjectId")
                    .HasColumnType("int");

                b.Property<string>("Role")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.Property<int>("UserId")
                    .HasColumnType("int");

                b.HasKey("ProjectMemberId");

                b.HasIndex("ProjectId");

                b.HasIndex("UserId");

                b.ToTable("ProjectMembers");

                b.HasData(
                    new
                    {
                        ProjectMemberId = 1,
                        AssignedDate = new DateTime(2026, 10, 20, 10, 24, 31, 386, DateTimeKind.Utc).AddTicks(0),
                        ProjectId = 1,
                        Role = "TeamLead",
                        UserId = 3
                    },
                    new
                    {
                        ProjectMemberId = 2,
                        AssignedDate = new DateTime(2026, 10, 20, 10, 24, 31, 386, DateTimeKind.Utc).AddTicks(0),
                        ProjectId = 1,
                        Role = "Developer",
                        UserId = 4
                    });
            });

            modelBuilder.Entity("ContosoDashboard.Models.Tag", b =>
            {
                b.Property<int>("TagId")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TagId"));

                b.Property<string>("Name")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.Property<string>("NormalizedName")
                    .IsRequired()
                    .HasColumnType("nvarchar(450)");

                b.HasKey("TagId");

                b.HasIndex("NormalizedName")
                    .IsUnique();

                b.ToTable("Tags");
            });

            modelBuilder.Entity("ContosoDashboard.Models.TaskComment", b =>
            {
                b.Property<int>("TaskCommentId")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TaskCommentId"));

                b.Property<string>("Comment")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.Property<DateTime>("CommentedDate")
                    .HasColumnType("datetime2");

                b.Property<int>("CommentedByUserId")
                    .HasColumnType("int");

                b.Property<int>("TaskId")
                    .HasColumnType("int");

                b.HasKey("TaskCommentId");

                b.HasIndex("CommentedByUserId");

                b.HasIndex("TaskId");

                b.ToTable("TaskComments");
            });

            modelBuilder.Entity("ContosoDashboard.Models.TaskDocument", b =>
            {
                b.Property<int>("TaskDocumentId")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TaskDocumentId"));

                b.Property<DateTime>("AttachedDate")
                    .HasColumnType("datetime2");

                b.Property<int>("AttachedByUserId")
                    .HasColumnType("int");

                b.Property<int>("DocumentId")
                    .HasColumnType("int");

                b.Property<int>("TaskId")
                    .HasColumnType("int");

                b.HasKey("TaskDocumentId");

                b.HasIndex("AttachedByUserId");

                b.HasIndex("DocumentId");

                b.HasIndex("TaskId");

                b.ToTable("TaskDocuments");
            });

            modelBuilder.Entity("ContosoDashboard.Models.TaskItem", b =>
            {
                b.Property<int>("TaskId")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TaskId"));

                b.Property<int>("AssignedUserId")
                    .HasColumnType("int");

                b.Property<int>("CreatedByUserId")
                    .HasColumnType("int");

                b.Property<DateTime>("CreatedDate")
                    .HasColumnType("datetime2");

                b.Property<string>("Description")
                    .HasColumnType("nvarchar(max)");

                b.Property<DateTime>("DueDate")
                    .HasColumnType("datetime2");

                b.Property<int>("Priority")
                    .HasColumnType("int");

                b.Property<int>("ProjectId")
                    .HasColumnType("int");

                b.Property<int>("Status")
                    .HasColumnType("int");

                b.Property<string>("Title")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.Property<DateTime>("UpdatedDate")
                    .HasColumnType("datetime2");

                b.HasKey("TaskId");

                b.HasIndex("AssignedUserId");

                b.HasIndex("CreatedByUserId");

                b.HasIndex("DueDate");

                b.HasIndex("ProjectId");

                b.HasIndex("Status");

                b.ToTable("Tasks");

                b.HasData(
                    new
                    {
                        TaskId = 1,
                        AssignedUserId = 4,
                        CreatedByUserId = 2,
                        CreatedDate = new DateTime(2026, 10, 20, 10, 24, 31, 386, DateTimeKind.Utc).AddTicks(0),
                        Description = "Create entity relationship diagram and database design",
                        DueDate = new DateTime(2026, 10, 30, 10, 24, 31, 386, DateTimeKind.Utc).AddTicks(0),
                        Priority = 2,
                        ProjectId = 1,
                        Status = 2,
                        Title = "Design database schema",
                        UpdatedDate = new DateTime(2026, 10, 30, 10, 24, 31, 386, DateTimeKind.Utc).AddTicks(0)
                    },
                    new
                    {
                        TaskId = 2,
                        AssignedUserId = 4,
                        CreatedByUserId = 2,
                        CreatedDate = new DateTime(2026, 10, 25, 10, 24, 31, 386, DateTimeKind.Utc).AddTicks(0),
                        Description = "Set up Microsoft Entra ID authentication",
                        DueDate = new DateTime(2026, 11, 24, 10, 24, 31, 386, DateTimeKind.Utc).AddTicks(0),
                        Priority = 3,
                        ProjectId = 1,
                        Status = 1,
                        Title = "Implement authentication",
                        UpdatedDate = new DateTime(2026, 11, 19, 10, 24, 31, 386, DateTimeKind.Utc).AddTicks(0)
                    },
                    new
                    {
                        TaskId = 3,
                        AssignedUserId = 4,
                        CreatedByUserId = 2,
                        CreatedDate = new DateTime(2026, 10, 30, 10, 24, 31, 386, DateTimeKind.Utc).AddTicks(0),
                        Description = "Design user interface mockups for all main pages",
                        DueDate = new DateTime(2026, 11, 29, 10, 24, 31, 386, DateTimeKind.Utc).AddTicks(0),
                        Priority = 1,
                        ProjectId = 1,
                        Status = 0,
                        Title = "Create UI mockups",
                        UpdatedDate = new DateTime(2026, 10, 30, 10, 24, 31, 386, DateTimeKind.Utc).AddTicks(0)
                    });
            });

            modelBuilder.Entity("ContosoDashboard.Models.User", b =>
            {
                b.Property<int>("UserId")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"));

                b.Property<int>("AvailabilityStatus")
                    .HasColumnType("int");

                b.Property<DateTime>("CreatedDate")
                    .HasColumnType("datetime2");

                b.Property<string>("Department")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.Property<string>("DisplayName")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.Property<string>("Email")
                    .IsRequired()
                    .HasColumnType("nvarchar(450)");

                b.Property<bool>("EmailNotificationsEnabled")
                    .HasColumnType("bit");

                b.Property<bool>("InAppNotificationsEnabled")
                    .HasColumnType("bit");

                b.Property<string>("JobTitle")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.Property<int>("Role")
                    .HasColumnType("int");

                b.HasKey("UserId");

                b.HasIndex("Email")
                    .IsUnique();

                b.ToTable("Users");

                b.HasData(
                    new
                    {
                        UserId = 1,
                        AvailabilityStatus = 0,
                        CreatedDate = new DateTime(2026, 11, 19, 10, 24, 31, 386, DateTimeKind.Utc).AddTicks(0),
                        Department = "IT",
                        DisplayName = "System Administrator",
                        Email = "admin@contoso.com",
                        EmailNotificationsEnabled = true,
                        InAppNotificationsEnabled = true,
                        JobTitle = "Administrator",
                        Role = 3
                    },
                    new
                    {
                        UserId = 2,
                        AvailabilityStatus = 0,
                        CreatedDate = new DateTime(2026, 11, 19, 10, 24, 31, 386, DateTimeKind.Utc).AddTicks(0),
                        Department = "Engineering",
                        DisplayName = "Camille Nicole",
                        Email = "camille.nicole@contoso.com",
                        EmailNotificationsEnabled = true,
                        InAppNotificationsEnabled = true,
                        JobTitle = "Project Manager",
                        Role = 2
                    },
                    new
                    {
                        UserId = 3,
                        AvailabilityStatus = 0,
                        CreatedDate = new DateTime(2026, 11, 19, 10, 24, 31, 386, DateTimeKind.Utc).AddTicks(0),
                        Department = "Engineering",
                        DisplayName = "Floris Kregel",
                        Email = "floris.kregel@contoso.com",
                        EmailNotificationsEnabled = true,
                        InAppNotificationsEnabled = true,
                        JobTitle = "Team Lead",
                        Role = 1
                    },
                    new
                    {
                        UserId = 4,
                        AvailabilityStatus = 0,
                        CreatedDate = new DateTime(2026, 11, 19, 10, 24, 31, 386, DateTimeKind.Utc).AddTicks(0),
                        Department = "Engineering",
                        DisplayName = "Ni Kang",
                        Email = "ni.kang@contoso.com",
                        EmailNotificationsEnabled = true,
                        InAppNotificationsEnabled = true,
                        JobTitle = "Software Engineer",
                        Role = 0
                    });
            });

            modelBuilder.Entity("ContosoDashboard.Models.Document", b =>
            {
                b.HasOne("ContosoDashboard.Models.Project", "Project")
                    .WithMany()
                    .HasForeignKey("ProjectId")
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasOne("ContosoDashboard.Models.User", "UploadedByUser")
                    .WithMany()
                    .HasForeignKey("UploadedByUserId")
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

                b.Navigation("Project");

                b.Navigation("UploadedByUser");
            });

            modelBuilder.Entity("ContosoDashboard.Models.DocumentActivity", b =>
            {
                b.HasOne("ContosoDashboard.Models.Document", "Document")
                    .WithMany("ActivityLog")
                    .HasForeignKey("DocumentId")
                    .OnDelete(DeleteBehavior.SetNull);

                b.HasOne("ContosoDashboard.Models.User", "PerformedByUser")
                    .WithMany()
                    .HasForeignKey("PerformedByUserId")
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

                b.Navigation("Document");

                b.Navigation("PerformedByUser");
            });

            modelBuilder.Entity("ContosoDashboard.Models.DocumentShare", b =>
            {
                b.HasOne("ContosoDashboard.Models.Document", "Document")
                    .WithMany("Shares")
                    .HasForeignKey("DocumentId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.HasOne("ContosoDashboard.Models.User", "SharedByUser")
                    .WithMany()
                    .HasForeignKey("SharedByUserId")
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

                b.HasOne("ContosoDashboard.Models.User", "SharedWithUser")
                    .WithMany()
                    .HasForeignKey("SharedWithUserId")
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

                b.Navigation("Document");

                b.Navigation("SharedByUser");

                b.Navigation("SharedWithUser");
            });

            modelBuilder.Entity("ContosoDashboard.Models.DocumentTag", b =>
            {
                b.HasOne("ContosoDashboard.Models.Document", "Document")
                    .WithMany("DocumentTags")
                    .HasForeignKey("DocumentId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.HasOne("ContosoDashboard.Models.Tag", "Tag")
                    .WithMany("DocumentTags")
                    .HasForeignKey("TagId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("Document");

                b.Navigation("Tag");
            });

            modelBuilder.Entity("ContosoDashboard.Models.Notification", b =>
            {
                b.HasOne("ContosoDashboard.Models.User", null)
                    .WithMany()
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity("ContosoDashboard.Models.Project", b =>
            {
                b.HasOne("ContosoDashboard.Models.User", "ProjectManager")
                    .WithMany("ManagedProjects")
                    .HasForeignKey("ProjectManagerId")
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

                b.Navigation("ProjectManager");
            });

            modelBuilder.Entity("ContosoDashboard.Models.ProjectMember", b =>
            {
                b.HasOne("ContosoDashboard.Models.Project", null)
                    .WithMany()
                    .HasForeignKey("ProjectId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.HasOne("ContosoDashboard.Models.User", null)
                    .WithMany()
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity("ContosoDashboard.Models.TaskComment", b =>
            {
                b.HasOne("ContosoDashboard.Models.User", "CommentedByUser")
                    .WithMany()
                    .HasForeignKey("CommentedByUserId")
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

                b.HasOne("ContosoDashboard.Models.TaskItem", null)
                    .WithMany()
                    .HasForeignKey("TaskId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("CommentedByUser");
            });

            modelBuilder.Entity("ContosoDashboard.Models.TaskDocument", b =>
            {
                b.HasOne("ContosoDashboard.Models.User", "AttachedByUser")
                    .WithMany()
                    .HasForeignKey("AttachedByUserId")
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

                b.HasOne("ContosoDashboard.Models.Document", null)
                    .WithMany("TaskDocuments")
                    .HasForeignKey("DocumentId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.HasOne("ContosoDashboard.Models.TaskItem", "Task")
                    .WithMany()
                    .HasForeignKey("TaskId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("AttachedByUser");

                b.Navigation("Task");
            });

            modelBuilder.Entity("ContosoDashboard.Models.TaskItem", b =>
            {
                b.HasOne("ContosoDashboard.Models.User", "AssignedUser")
                    .WithMany("AssignedTasks")
                    .HasForeignKey("AssignedUserId")
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

                b.HasOne("ContosoDashboard.Models.User", "CreatedByUser")
                    .WithMany("CreatedTasks")
                    .HasForeignKey("CreatedByUserId")
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

                b.HasOne("ContosoDashboard.Models.Project", null)
                    .WithMany()
                    .HasForeignKey("ProjectId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("AssignedUser");

                b.Navigation("CreatedByUser");
            });

            modelBuilder.Entity("ContosoDashboard.Models.Document", b =>
            {
                b.Navigation("ActivityLog");

                b.Navigation("DocumentTags");

                b.Navigation("Shares");

                b.Navigation("TaskDocuments");
            });

            modelBuilder.Entity("ContosoDashboard.Models.Tag", b =>
            {
                b.Navigation("DocumentTags");
            });

            modelBuilder.Entity("ContosoDashboard.Models.User", b =>
            {
                b.Navigation("AssignedTasks");

                b.Navigation("CreatedTasks");

                b.Navigation("ManagedProjects");
            });
#pragma warning restore 612, 618
        }
    }
}
