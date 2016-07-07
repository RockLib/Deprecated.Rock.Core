using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Validation;
using ManyConsole;
using Rock.StringFormatting;

namespace SampleApplication
{
    public class EntityFrameworkValidationErrorCommand : ConsoleCommand
    {
        public EntityFrameworkValidationErrorCommand()
        {
            this.IsCommand("EntityFrameworkValidationError");
            this.SkipsCommandSummaryBeforeRunning();
        }

        public override int Run(string[] remainingArguments)
        {
            using (var db = new WidgetContext())
            {
                var widget = new Widget
                {
                    Value = "foo",
                    Gadgets = new List<Gadget>
                    {
                        new Gadget
                        {
                            Value = 30
                        }
                    }
                };

                db.Widgets.Add(widget);

                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    var formattedException = ex.FormatToString();
                    Console.WriteLine(formattedException);
                }
            }

            return 0;
        }
    }

    public class Widget
    {
        public int WidgetId { get; set; }

        [Required]
        public string Name { get; set; }

        [StringLength(10, MinimumLength = 5)]
        public string Value { get; set; }

        public virtual List<Gadget> Gadgets { get; set; } 
    }

    public class Gadget
    {
        public int GadgetId { get; set; }

        [Range(10, 20)]
        public int Value { get; set; }

        public virtual Widget Widget { get; set; }
    }

    /*public class Blog
    {
        public int BlogId { get; set; }

        [Required]
        public string Name { get; set; }

        public virtual List<Post> Posts { get; set; }
    }

    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public int BlogId { get; set; }
        public virtual Blog Blog { get; set; }
    }*/

    public class WidgetContext : DbContext
    {
        public DbSet<Widget> Widgets { get; set; }
    }
}