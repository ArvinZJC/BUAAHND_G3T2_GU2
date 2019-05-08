#region Using Directives
using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
#endregion Using Directives

namespace NewEraFlowerStore.Data
{
    public class DbInitialiser
    {
        public static async void InitialiseAsync(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            #region Deleting Old Data
            // delete colour data and bouquet data if there is any colour data
            if (context.Colours.Any())
            {
                context.Colours.RemoveRange(await context.Colours.ToListAsync());
                await context.SaveChangesAsync();
            } // end if

            // delete flower data if there is any
            if (context.Flowers.Any())
            {
                context.Flowers.RemoveRange(await context.Flowers.ToListAsync());
                await context.SaveChangesAsync();
            } // end if

            // delete occasion data if there is any
            if (context.Occasions.Any())
            {
                context.Occasions.RemoveRange(await context.Occasions.ToListAsync());
                await context.SaveChangesAsync();
            } // end if

            // if there is any Identity role data, find and delete the data whose name is not "Administrator" or "Customer"
            if (context.Roles.Any())
            {
                foreach (var role in context.Roles)
                    if (!string.Equals(role.Name, "Administrator", StringComparison.OrdinalIgnoreCase) && !string.Equals(role.Name, "Customer", StringComparison.OrdinalIgnoreCase))
                        context.Roles.Remove(role);

                await context.SaveChangesAsync();
            } // end if

            // delete order data if there is any
            if (context.Orders.Any())
            {
                context.Orders.RemoveRange(await context.Orders.ToListAsync());
                await context.SaveChangesAsync();
            } // end if

            // delete sales record data if there is any
            if (context.SalesRecords.Any())
            {
                context.SalesRecords.RemoveRange(await context.SalesRecords.ToListAsync());
                await context.SaveChangesAsync();
            } // end if
            #endregion Deleting Old Data

            #region Adding Identity Role Data
            string[] roleNames = { "Administrator", "Customer" };
            IdentityResult createRoleResult;

            foreach (var roleName in roleNames)
            {
                // create the specified role if it does not exist
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    createRoleResult = await roleManager.CreateAsync(new IdentityRole(roleName));

                    if (!createRoleResult.Succeeded)
                        throw new InvalidOperationException($"Error! Failed to create the specified Identity role.");
                } // end if
            } // end foreach
            #endregion Adding Identity Role Data

            IdentityResult createUserResult, addToRoleResult, removeFromRoleResult;

            #region Adding Administrator Data
            #region Administrator 1
            var administrator = await userManager.FindByEmailAsync("nefsstaff1@shieldadmin.com");

            if (administrator == null)
            {
                administrator = new ApplicationUser()
                {
                    AvatarUrl = string.Empty,
                    FirstName = "Arvin",
                    LastName = "Zhao",
                    UserName = "NefsStaff1",
                    Email = "nefsstaff1@shieldadmin.com",
                    EmailConfirmed = true,
                    PhoneNumber = "11111111111",
                    RegistrationTime = DateTimeOffset.Now
                };
                createUserResult = await userManager.CreateAsync(administrator, "nefsstaff1");

                if (createUserResult.Succeeded)
                {
                    addToRoleResult = await userManager.AddToRoleAsync(administrator, "Administrator");

                    if (!addToRoleResult.Succeeded)
                        throw new InvalidOperationException($"Error! Failed to add the specified administrator to the Administrator role.");
                }
                else
                    throw new InvalidOperationException($"Error! Failed to create the specified administrator.");
            }
            else
            {
                var accountRoles = await userManager.GetRolesAsync(administrator);

                if (accountRoles.Contains("Customer"))
                {
                    removeFromRoleResult = await userManager.RemoveFromRoleAsync(administrator, "Customer");

                    if (!removeFromRoleResult.Succeeded)
                        throw new InvalidOperationException($"Error! Failed to remove the Customer role from the specified administrator.");
                } // end if

                if (!accountRoles.Contains("Administrator"))
                {
                    addToRoleResult = await userManager.AddToRoleAsync(administrator, "Administrator");

                    if (!addToRoleResult.Succeeded)
                        throw new InvalidOperationException($"Error! Failed to add the specified administrator to the Administrator role.");
                } // end if
            } // end if...else
            #endregion Administrator 1

            #region Administrator 2
            administrator = await userManager.FindByEmailAsync("nefsstaff2@shieldadmin.com");

            if (administrator == null)
            {
                administrator = new ApplicationUser()
                {
                    AvatarUrl = string.Empty,
                    FirstName = "Mary",
                    LastName = "Air",
                    UserName = "NefsStaff2",
                    Email = "nefsstaff2@shieldadmin.com",
                    EmailConfirmed = true,
                    PhoneNumber = "22222222222",
                    RegistrationTime = DateTimeOffset.Now
                };
                createUserResult = await userManager.CreateAsync(administrator, "nefsstaff2");

                if (createUserResult.Succeeded)
                {
                    addToRoleResult = await userManager.AddToRoleAsync(administrator, "Administrator");

                    if (!addToRoleResult.Succeeded)
                        throw new InvalidOperationException($"Error! Failed to add the specified administrator to the Administrator role.");
                }
                else
                    throw new InvalidOperationException($"Error! Failed to create the specified administrator.");
            }
            else
            {
                var accountRoles = await userManager.GetRolesAsync(administrator);

                if (accountRoles.Contains("User"))
                {
                    removeFromRoleResult = await userManager.RemoveFromRoleAsync(administrator, "Customer");

                    if (!removeFromRoleResult.Succeeded)
                        throw new InvalidOperationException($"Error! Failed to remove the Customer role from the specified administrator.");
                } // end if

                if (!accountRoles.Contains("Administrator"))
                {
                    addToRoleResult = await userManager.AddToRoleAsync(administrator, "Administrator");

                    if (!addToRoleResult.Succeeded)
                        throw new InvalidOperationException($"Error! Failed to add the specified administrator to the Administrator role.");
                } // end if
            } // end if...else
            #endregion Administrator 2

            #region Administrator 3
            administrator = await userManager.FindByEmailAsync("nefsstaff3@shieldadmin.com");

            if (administrator == null)
            {
                administrator = new ApplicationUser()
                {
                    AvatarUrl = string.Empty,
                    FirstName = "Peter",
                    LastName = "Tab",
                    UserName = "NefsStaff3",
                    Email = "nefsstaff3@shieldadmin.com",
                    EmailConfirmed = true,
                    PhoneNumber = "33333333333",
                    RegistrationTime = DateTimeOffset.Now
                };
                createUserResult = await userManager.CreateAsync(administrator, "nefsstaff3");

                if (createUserResult.Succeeded)
                {
                    addToRoleResult = await userManager.AddToRoleAsync(administrator, "Administrator");

                    if (!addToRoleResult.Succeeded)
                        throw new InvalidOperationException($"Error! Failed to add the specified administrator to the Administrator role.");
                }
                else
                    throw new InvalidOperationException($"Error! Failed to create the specified administrator.");
            }
            else
            {
                var accountRoles = await userManager.GetRolesAsync(administrator);

                if (accountRoles.Contains("User"))
                {
                    removeFromRoleResult = await userManager.RemoveFromRoleAsync(administrator, "Customer");

                    if (!removeFromRoleResult.Succeeded)
                        throw new InvalidOperationException($"Error! Failed to remove the Customer role from the specified administrator.");
                } // end if

                if (!accountRoles.Contains("Administrator"))
                {
                    addToRoleResult = await userManager.AddToRoleAsync(administrator, "Administrator");

                    if (!addToRoleResult.Succeeded)
                        throw new InvalidOperationException($"Error! Failed to add the specified administrator to the Administrator role.");
                } // end if
            } // end if...else
            #endregion Administrator 3
            #endregion Adding Administrator Data

            #region Adding Customer Data
            #region Customer 1
            var customer = await userManager.FindByEmailAsync("testcustomer1@nefs.com");

            if (customer == null)
            {
                customer = new ApplicationUser()
                {
                    AvatarUrl = "_default.jpg",
                    FirstName = "Mark",
                    LastName = "Zaker",
                    UserName = "TestCustomer1",
                    Email = "testcustomer1@nefs.com",
                    EmailConfirmed = true,
                    RegistrationTime = DateTimeOffset.Now.AddDays(-25)
                };
                createUserResult = await userManager.CreateAsync(customer, "testcustomer1");

                if (createUserResult.Succeeded)
                {
                    addToRoleResult = await userManager.AddToRoleAsync(customer, "Customer");

                    if (!addToRoleResult.Succeeded)
                        throw new InvalidOperationException($"Error! Failed to add the specified customer to the Customer role.");
                }
                else
                    throw new InvalidOperationException($"Error! Failed to create the specified customer.");
            }
            else
            {
                customer.RegistrationTime = DateTimeOffset.Now.AddDays(-25);

                var result = await userManager.UpdateAsync(customer);

                if (!result.Succeeded)
                    throw new InvalidOperationException($"Error! Failed to update registration time of the specified customer.");

                var accountRoles = await userManager.GetRolesAsync(customer);

                if (accountRoles.Contains("Administrator"))
                {
                    removeFromRoleResult = await userManager.RemoveFromRoleAsync(customer, "Administrator");

                    if (!removeFromRoleResult.Succeeded)
                        throw new InvalidOperationException($"Error! Failed to remove the Administrator role from the specified customer.");
                } // end if

                if (!accountRoles.Contains("Customer"))
                {
                    addToRoleResult = await userManager.AddToRoleAsync(administrator, "Customer");

                    if (!addToRoleResult.Succeeded)
                        throw new InvalidOperationException($"Error! Failed to add the specified customer to the Customer role.");
                } // end if
            } // end if...else
            #endregion Customer 1

            #region Customer 2
            customer = await userManager.FindByEmailAsync("testcustomer2@nefs.com");

            if (customer == null)
            {
                customer = new ApplicationUser()
                {
                    AvatarUrl = "_default.jpg",
                    FirstName = "May",
                    LastName = "Cute",
                    UserName = "TestCustomer2",
                    Email = "testcustomer2@nefs.com",
                    EmailConfirmed = true,
                    RegistrationTime = DateTimeOffset.Now.AddDays(-10)
                };
                createUserResult = await userManager.CreateAsync(customer, "testcustomer2");

                if (createUserResult.Succeeded)
                {
                    addToRoleResult = await userManager.AddToRoleAsync(customer, "Customer");

                    if (!addToRoleResult.Succeeded)
                        throw new InvalidOperationException($"Error! Failed to add the specified customer to the Customer role.");
                }
                else
                    throw new InvalidOperationException($"Error! Failed to create the specified customer.");
            }
            else
            {
                customer.RegistrationTime = DateTimeOffset.Now.AddDays(-10);

                var result = await userManager.UpdateAsync(customer);

                if (!result.Succeeded)
                    throw new InvalidOperationException($"Error! Failed to update registration time of the specified customer.");

                var accountRoles = await userManager.GetRolesAsync(customer);

                if (accountRoles.Contains("Administrator"))
                {
                    removeFromRoleResult = await userManager.RemoveFromRoleAsync(customer, "Administrator");

                    if (!removeFromRoleResult.Succeeded)
                        throw new InvalidOperationException($"Error! Failed to remove the Administrator role from the specified customer.");
                } // end if

                if (!accountRoles.Contains("Customer"))
                {
                    addToRoleResult = await userManager.AddToRoleAsync(administrator, "Customer");

                    if (!addToRoleResult.Succeeded)
                        throw new InvalidOperationException($"Error! Failed to add the specified customer to the Customer role.");
                } // end if
            } // end if...else
            #endregion Customer 2

            #region Customer 3
            customer = await userManager.FindByEmailAsync("testcustomer3@nefs.com");

            if (customer == null)
            {
                customer = new ApplicationUser()
                {
                    AvatarUrl = "_default.jpg",
                    FirstName = "Ben",
                    LastName = "Yvonne",
                    UserName = "TestCustomer3",
                    Email = "testcustomer3@nefs.com",
                    EmailConfirmed = true,
                    RegistrationTime = DateTimeOffset.Now.AddDays(-5)
                };
                createUserResult = await userManager.CreateAsync(customer, "testcustomer3");

                if (createUserResult.Succeeded)
                {
                    addToRoleResult = await userManager.AddToRoleAsync(customer, "Customer");

                    if (!addToRoleResult.Succeeded)
                        throw new InvalidOperationException($"Error! Failed to add the specified customer to the Customer role.");
                }
                else
                    throw new InvalidOperationException($"Error! Failed to create the specified customer.");
            }
            else
            {
                customer.RegistrationTime = DateTimeOffset.Now.AddDays(-5);

                var result = await userManager.UpdateAsync(customer);

                if (!result.Succeeded)
                    throw new InvalidOperationException($"Error! Failed to update registration time of the specified customer.");

                var accountRoles = await userManager.GetRolesAsync(customer);

                if (accountRoles.Contains("Administrator"))
                {
                    removeFromRoleResult = await userManager.RemoveFromRoleAsync(customer, "Administrator");

                    if (!removeFromRoleResult.Succeeded)
                        throw new InvalidOperationException($"Error! Failed to remove the Administrator role from the specified customer.");
                } // end if

                if (!accountRoles.Contains("Customer"))
                {
                    addToRoleResult = await userManager.AddToRoleAsync(administrator, "Customer");

                    if (!addToRoleResult.Succeeded)
                        throw new InvalidOperationException($"Error! Failed to add the specified customer to the Customer role.");
                } // end if
            } // end if...else
            #endregion Customer 3
            #endregion Adding Customer Data

            #region Adding Colour Data
            var colours = new Colour[]
            {
                new Colour{ID = 1, Name = "Assorted"},
                new Colour{ID = 2, Name = "Blue"},
                new Colour{ID = 3, Name = "Green"},
                new Colour{ID = 4, Name = "Orange"},
                new Colour{ID = 5, Name = "Red"},
                new Colour{ID = 6, Name = "Pink"},
                new Colour{ID = 7, Name = "Purple"},
                new Colour{ID = 8, Name = "White"},
                new Colour{ID = 9, Name = "Yellow"}
            };

            foreach (var item in colours)
                context.Colours.Add(item);

            await context.SaveChangesAsync();
            #endregion Adding Colour Data

            #region Adding Flower Data
            var flowers = new Flower[]
            {
                new Flower
                {
                    ID = 1,
                    Name = "Daisies",
                    Description = "There is nothing quite as cheerful as sending daisies. When you gather up a bunch and arrange them in a " +
                        "colourful bouquet, it is like delivering a box of sunshine.",
                    CoverPhotoUrl = "1.jpg"
                },
                new Flower
                {
                    ID = 2,
                    Name = "Irises",
                    Description = "The iris is one of the most vibrant blooms we offer, adding a punch of colour to any bouquet. Send the beauty " +
                        "of these special iris flowers to the home of a friend or loved one, and watch their spirits soar.",
                    CoverPhotoUrl = "2.jpg"
                },
                new Flower
                {
                    ID = 3,
                    Name = "Lilies",
                    Description = "One of the most fragrant flowers, lilies come in all shapes in colours, from our rainbows of Peruvian lilies " +
                        "to our elegant and bold Stargazers.",
                    CoverPhotoUrl = "3.jpg"
                },
                new Flower
                {
                    ID = 4,
                    Name = "Orchids & tropicals",
                    Description = "One of the most elegant flowers, the orchid is strikingly exotic and instantly takes any table top in the home" +
                        " from boring to chic.",
                    CoverPhotoUrl = "4.jpg"
                },
                new Flower
                {
                    ID = 5,
                    Name = "Roses",
                    Description = "Roses do not just mean romance! Choose a classic all-rose bouquet for a loved one, or select a multi-coloured " +
                        "mixed bouquet for family, friends, and even colleagues! Fragrant roses are always a welcome surprise.",
                    CoverPhotoUrl = "5.jpg"
                },
                new Flower
                {
                    ID = 6,
                    Name = "Sunflowers",
                    Description = "Showy, sturdy, and sure to please, the magnificent blooms on our sunflower bouquets bring good cheer to any " +
                        "setting. Send sunflowers and really brighten a room or, better, a heart.",
                    CoverPhotoUrl = "6.jpg"
                },
                new Flower
                {
                    ID = 7,
                    Name = "Tulips",
                    Description = "When it comes to fresh tulips, we are better in the business. Our tulips arrive in bud form to provide maximum bloom time once they are received. Any fresher and you would have to pick them yourself.",
                    CoverPhotoUrl = "7.jpg"
                }
            };

            foreach (var item in flowers)
                context.Flowers.Add(item);

            await context.SaveChangesAsync();
            #endregion Adding Flower Data

            #region Adding Occasion Data
            var occasions = new Occasion[]
            {
                new Occasion
                {
                    ID = 1,
                    Name = "Anniversary",
                    Description = "Mark the milestones with anniversary bouquets that shows you still choose them.",
                    CoverPhotoUrl = "1.jpg"
                },
                new Occasion
                {
                    ID = 2,
                    Name = "Baby & kids gifts",
                    Description = "Say \"way to go, mom and dad\" with a big bouquet of new baby flowers. In addition to those, we have all great" +
                        " bouquets as gifts to kids.",
                    CoverPhotoUrl = "2.jpg"
                },
                new Occasion
                {
                    ID = 3,
                    Name = "Birthday",
                    Description = "A birthday delivery of fresh, fragrant bouquets is an easy and personal way to let someone know you remembered" +
                        " their special day. Our wide variety of birthday flower bouquets is always the right choice for every birthday on your " +
                        "list.",
                    CoverPhotoUrl = "3.jpg"
                },
                new Occasion
                {
                    ID = 4,
                    Name = "Celebration",
                    Description = "She takes her curtain call, he graduates, they get married, we enjoy festivals, and there are flowers in their hands. With a " +
                        "bouquet of celebration flowers, any event is more colourful and memorable.",
                    CoverPhotoUrl = "4.jpg"
                },
                new Occasion
                {
                    ID = 5,
                    Name = "Corporate gifts",
                    Description = "Corporate gifts can be perfectly appropriate, and still perfectly thoughtful. Our bouquets will show your " +
                        "genuine appreciation for anyone in your workplace.",
                    CoverPhotoUrl = "5.jpg"
                },
                new Occasion
                {
                    ID = 6,
                    Name = "Funeral",
                    Description = "For a floral tribute that blooms with breath-taking beauty, look no further. Send funeral flowers with our " +
                        "unbeatable selection of freshly arranged flowers for funerals and memorial services.",
                    CoverPhotoUrl = "6.jpg"
                },
                new Occasion
                {
                    ID = 7,
                    Name = "Get well",
                    Description = "When a loved one is under the weather, any gesture letting them know you care can mean the world. A bright, " +
                        "fragrant \"get well\" bouquet of flowers works wonders for lifting spirits and improving their outlook.",
                    CoverPhotoUrl = "7.jpg"
                },
                new Occasion
                {
                    ID = 8,
                    Name = "Housewarming",
                    Description = "What better way to christen a new home than with the gift of a charming bouquet? Our housewarming bouquets " +
                        "will not only brighten their new space, but the fragrant blooms will have it feeling like home in no time.",
                    CoverPhotoUrl = "8.jpg"
                },
                new Occasion
                {
                    ID = 9,
                    Name = "Just because",
                    Description = "Why wait for a birthday, a holiday, or an anniversary to make someone feel special? We think that one of the " +
                        "best reasons to send a gorgeous bouquet is, well... just because.",
                    CoverPhotoUrl = "9.jpg"
                },
                new Occasion
                {
                    ID = 10,
                    Name = "Love & romance",
                    Description = "While some romantic moments simply cannot be planned, others can be delivered to her doorstep on your day of " +
                        "choice. Our romantic bouquets will remind her that celebrating your feelings for each other does not require a special " +
                        "occasion.",
                    CoverPhotoUrl = "10.jpg"
                },
                new Occasion
                {
                    ID = 11,
                    Name = "Sympathy",
                    Description = "The most popular flowers to express your sympathy include lilies, roses, orchids, etc., which is why we have " +
                        "arranged them together to create stunning sympathy bouquets. During a time when words will not suffice, a simple bouquet" +
                        " of sympathy flowers can lift a loved one's spirits.",
                    CoverPhotoUrl = "11.jpg"
                },
                new Occasion
                {
                    ID = 12,
                    Name = "Thank you",
                    Description = "When you want to show appreciation and a note just will not do, opt for thoughtful thank you bouquets. It is " +
                        "the perfect way to express your gratitude with style.",
                    CoverPhotoUrl = "12.jpg"
                }
            };

            foreach (var item in occasions)
                context.Occasions.Add(item);

            await context.SaveChangesAsync();
            #endregion Adding Occasion Data

            #region Adding Bouquet Data
            var bouquets = new Bouquet[]
            {
                new Bouquet
                {
                    ID = 1,
                    Name = "Colourful Gerbera Daisies",
                    PhotoUrl1 = "1-1.jpg",
                    PhotoUrl2 = "1-2.jpg",
                    Description = "Celebrate an anniversary with a bright, beautiful Gerbera daisy bouquet. Available in a rainbow of " +
                        "colours, Gerbera daisies are fast becoming a favorite of flower lovers. Our Gerbera daisies are prized for their " +
                        "long wavy stems and big, bold blooms.\r\n" +
                        "• 15 stems Gerbera daisies\r\n" +
                        "• Yellow filler\r\n" +
                        "• Greenery\r\n" +
                        "• Stands approximately 16 inches tall\r\n" +
                        "• Ships in custom packaging and gift box",
                    LaunchDate = DateTime.Now.Date.AddMonths(-1),
                    ColourId = 1,
                    FlowerId = 1,
                    OccasionId = 1,
                    OriginalPrice = 49.99M,
                    Discount = 0M,
                    Stocks = 123,
                    Sales = 555
                },
                new Bouquet
                {
                    ID = 2,
                    Name = "100 Blooms of Valentine's Day Wishes😘",
                    PhotoUrl1 = "2-1.jpg",
                    PhotoUrl2 = "2-2.jpg",
                    Description = "You want to really win someone over this Valentine's Day? Make her every wish come true - send a lavish" +
                        " bouquet of lavender daisies, white poms and pink and red carnations to her, and she is sure to be all yours.\r\n" +
                        "• 4 white poms\r\n" +
                        "• 3 lavender daisies\r\n" +
                        "• 3 light pink mini carnations\r\n" +
                        "• 2 red mini carnations\r\n" +
                        "• 3 red carnations\r\n" +
                        "• 5 white or lavender butterfly asters\r\n" +
                        "• Ships in custom packaging and gift box",
                    LaunchDate = DateTime.Parse("2019-2-10"),
                    ColourId = 1,
                    FlowerId = 1,
                    OccasionId = 10,
                    OriginalPrice = 39.99M,
                    Discount = 0.40M,
                    Stocks = 2,
                    Sales = 1314
                },
                new Bouquet
                {
                    ID = 3,
                    Name = "Joyful Bouquet",
                    PhotoUrl1 = "3-1.jpg",
                    PhotoUrl2 = "3-2.jpg",
                    Description = "This mixed bouquet is sure to impress! It is crafted from our ever popular Stargazer lilies and blue " +
                        "iris, and is a spectacular way to celebrate an anniversary! And you will not find this at your supermarket or in" +
                        " your florist's cooler: flowers this fresh and vibrant are only at our store.\r\n" +
                        "• 3 stems of Stargazer lilies (2-4 blooms per stem)\r\n" +
                        "• 10 blue iris\r\n" +
                        "• Stands approximately 20 inches tall\r\n" +
                        "• Ships in custom packaging and gift box",
                    LaunchDate = DateTime.Now.Date.AddDays(-20.0),
                    ColourId = 1,
                    FlowerId = 2,
                    OccasionId = 7,
                    OriginalPrice = 29.99M,
                    Discount = 0M,
                    Stocks = 19,
                    Sales = 1001
                },
                new Bouquet
                {
                    ID = 4,
                    Name = "Blue Moon Roses",
                    PhotoUrl1 = "4-1.jpg",
                    PhotoUrl2 = "4-2.jpg",
                    Description = "They will get lost into the night staring deep into the bold and mesmerising blue colour of these long" +
                        " stemmed roses.\r\n" +
                        "• 1 dozen long stemmed blue dyed roses\r\n" +
                        "• Stands approximately 20 inches tall\r\n" +
                        "• Ships in custom packaging and gift box",
                    LaunchDate = DateTime.Now.Date.AddDays(-80.0),
                    ColourId = 2,
                    FlowerId = 5,
                    OccasionId = 1,
                    OriginalPrice = 49.99M,
                    Discount = 0.05M,
                    Stocks = 27,
                    Sales = 1002
                },
                new Bouquet
                {
                    ID = 5,
                    Name = "Easter Blooms",
                    PhotoUrl1 = "5-1.jpg",
                    PhotoUrl2 = "5-2.jpg",
                    Description = "A dizzying array of deep, robust colour, the Easter Blooms bouquet contains 12 blue iris, fresh from " +
                        "the fields. It is an always-impressive bouquet perfect for a \"dinner for two\", a special someone's birthday, or" +
                        " anytime you want to make someone's heart race.\r\n" +
                        "• 1 dozen Blue iris with some red and pink tulips\r\n" +
                        "• Stands approximately 16 inches tall\r\n" +
                        "• Ships in custom packaging and gift box",
                    LaunchDate = DateTime.Parse("2019-4-5"),
                    ColourId = 2,
                    FlowerId = 2,
                    OccasionId = 4,
                    OriginalPrice = 39.99M,
                    Discount = 0.15M,
                    Stocks = 111,
                    Sales = 222
                },
                new Bouquet
                {
                    ID = 6,
                    Name = "Carnival of Colour",
                    PhotoUrl1 = "6-1.jpg",
                    PhotoUrl2 = "6-2.jpg",
                    Description = "A bouquet of outrageous beauty, our Carnival of Colour bouquet offers a playful collection of " +
                        "vibrant-toned flowers: orange lilies (main flower), burgundy asters, blue iris/purple asters, and so much more. " +
                        "A fun gift for a fun friend or anyone who is the life of the party.\r\n" +
                        "• 4 stems orange Asiatic lilies (2-4 blooms per stem)\r\n" +
                        "• 4 purple liatris\r\n" +
                        "• 5 blue iris/purple asters\r\n" +
                        "• 4 orange/yellow germinis\r\n" +
                        "• 3 burgundy Matsumoto asters\r\n" +
                        "• 3 lemon leaf\r\n" +
                        "• Stands approximately 20 inches tall\r\n" +
                        "• Ships in custom packaging and gift box",
                    LaunchDate = DateTime.Now.Date.AddDays(-10.0),
                    ColourId = 4,
                    FlowerId = 3,
                    OccasionId = 4,
                    OriginalPrice = 69.99M,
                    Discount = 0.35M,
                    Stocks = 100,
                    Sales = 23
                },
                new Bouquet
                {
                    ID = 7,
                    Name = "Sunflower Radiance",
                    PhotoUrl1 = "7-1.jpg",
                    PhotoUrl2 = "7-2.jpg",
                    Description = "Send sunflowers and a little sunshine to brighten up someone's day! Gorgeous and always cheerful, these" +
                        " seven yellow sunflowers are shipped in \"bud\" form, to ensure they will reach full bloom after arrival.\r\n" +
                        "• 7 sunflowers\r\n" +
                        "• Lush greenery\r\n" +
                        "• Stands approximately 14 inches tall\r\n" +
                        "• Ships in custom packaging and gift box",
                    LaunchDate = DateTime.Now.Date.AddDays(-11.0),
                    ColourId = 9,
                    FlowerId = 6,
                    OccasionId = 2,
                    OriginalPrice = 39.99M,
                    Discount = 0.02M,
                    Stocks = 222,
                    Sales = 1003
                },
                new Bouquet
                {
                    ID = 8,
                    Name = "36 Pink Pearl Roses",
                    PhotoUrl1 = "8-1.jpg",
                    PhotoUrl2 = "8-2.jpg",
                    Description = "Lovely and luxurious, Pink Pearl Roses are the perfect gift for the Princess in your life. Fuller, " +
                        "fabulous, and long-lasting, this arrangement brings elegance and charm to any occasion.\r\n" +
                        "• 36 bi-colour pink roses\r\n" +
                        "• Stands approximately 16 inches tall\r\n" +
                        "• Ships in custom packaging and gift box",
                    LaunchDate = DateTime.Now.Date.AddDays(-12.0),
                    ColourId = 6,
                    FlowerId = 5,
                    OccasionId = 2,
                    OriginalPrice = 79.99M,
                    Discount = 0.25M,
                    Stocks = 333,
                    Sales = 1004
                },
                new Bouquet
                {
                    ID = 9,
                    Name = "2 Dozen Long Stemmed Red Birthday Roses",
                    PhotoUrl1 = "9-1.jpg",
                    PhotoUrl2 = "9-2.jpg",
                    Description = "Roses: the definitive expression of lasting love! Send 2-dozen long-stemmed roses and send an " +
                        "unmistakable message. These long-stemmed roses are cut, shipped, and packed within days, so when they arrive at " +
                        "your beloved's door, they will remain bold and beautiful for at least 7 days - guaranteed.\r\n" +
                        "• 24 long-stemmed red roses\r\n" +
                        "• Includes greenery\r\n" +
                        "• Stands approximately 20 inches tall\r\n" +
                        "• Ships in custom packaging and gift box",
                    LaunchDate = DateTime.Now.Date.AddDays(-40.0),
                    ColourId = 5,
                    FlowerId = 5,
                    OccasionId = 3,
                    OriginalPrice = 49.99M,
                    Discount = 0.30M,
                    Stocks = 444,
                    Sales = 777
                },
                new Bouquet
                {
                    ID = 10,
                    Name = "Birthday Lilies",
                    PhotoUrl1 = "10-1.jpg",
                    PhotoUrl2 = "10-2.jpg",
                    Description = "Beautiful Royal Birthday Lilies are just the flower to welcome spring! In wonderful hues of peach, pink" +
                    ", and yellow, these lovely blooms will add sunshine to any room. So that your flowers last their longest, they are " +
                    "shipped fresh, budding, and ready to bloom.\r\n" +
                    "• 12 stems of pink, peach, and yellow Royal lilies (2-4 blooms per stem)\r\n" +
                    "• Measures approximately 20 inches tall\r\n" +
                    "• Ships in custom packaging and gift box",
                    LaunchDate = DateTime.Now.Date.AddDays(-50.0),
                    ColourId = 9,
                    FlowerId = 3,
                    OccasionId = 3,
                    OriginalPrice = 29.99M,
                    Discount = 0.10M,
                    Stocks = 555,
                    Sales = 666
                },
                new Bouquet
                {
                    ID = 11,
                    Name = "White Dendrobium Orchids",
                    PhotoUrl1 = "11-1.jpg",
                    PhotoUrl2 = "11-2.jpg",
                    Description = "These 15 stems of white Dendrobium orchid have multiple butterfly-shaped blooms, and create an " +
                        "impressive display wherever they are placed. As a business gift or as an expression of admiration or sympathy, " +
                        "we will put our white orchids up against any other flower or gift on the market.\r\n" +
                        "• 15 multiple bloom stems white Dendrobium orchids\r\n" +
                        "• Stands at least 14 inches tall\r\n" +
                        "• Imported from Thailand\r\n" +
                        "• Ships in custom packaging and gift box",
                    LaunchDate = DateTime.Now.Date.AddDays(-70.0),
                    ColourId = 8,
                    FlowerId = 4,
                    OccasionId = 5,
                    OriginalPrice = 39.99M,
                    Discount = 0.15M,
                    Stocks = 9,
                    Sales = 555
                },
                new Bouquet
                {
                    ID = 12,
                    Name = "Callas in Dark Purple",
                    PhotoUrl1 = "12-1.jpg",
                    PhotoUrl2 = "12-2.jpg",
                    Description = "They say wearing dark purple is a lifestyle. These stems of dark and gorgeous callas is a style must " +
                        "have for any room.\r\n" +
                        "• 35 Dark Purple Callas\r\n" +
                        "• Stands approximately 20 inches tall\r\n" +
                        "• Ships in custom packaging and gift box",
                    LaunchDate = DateTime.Now.Date.AddDays(-5.0),
                    ColourId = 7,
                    FlowerId = 3,
                    OccasionId = 5,
                    OriginalPrice = 99.99M,
                    Discount = 0.50M,
                    Stocks = 290,
                    Sales = 233
                },
                new Bouquet
                {
                    ID = 13,
                    Name = "Sympathy Peace Lily with Angel",
                    PhotoUrl1 = "13-1.jpg",
                    PhotoUrl2 = "13-2.jpg",
                    Description = "Paying your respects is never easy. But the dark green leaves and white flag-like flowers of our " +
                        "Sympathy Peace Lily enable you to easily deliver a tender, heartfelt expression of sympathy and respect. This " +
                        "easy-care plant will brighten any location. A keepsake white ceramic angel completes the gift.\r\n" +
                        "• Includes our elegant Peace lily (Spathiphyllum)\r\n" +
                        "• Arrives with multiple blooms\r\n" +
                        "• Gift stands approximately 20-22 inches tall and ready to bloom\r\n" +
                        "• Includes our exclusively designed ceramic dove cross vase with gold accents\r\n" +
                        "• Includes white ceramic angel keepsake\r\n" +
                        "• Delivered with your personal message and care instructions",
                    LaunchDate = DateTime.Now.Date.AddDays(-9.0),
                    ColourId = 3,
                    FlowerId = 3,
                    OccasionId = 11,
                    OriginalPrice = 39.99M,
                    Discount = 0.15M,
                    Stocks = 999,
                    Sales = 332
                },
                new Bouquet
                {
                    ID = 14,
                    Name = "Peace and Prayers",
                    PhotoUrl1 = "14-1.jpg",
                    PhotoUrl2 = "14-2.jpg",
                    Description = "An appropriate and beautiful remembrance. 5 pristine white roses stand out amongst eucalyptus and " +
                        "delicate green dianthus for a quiet serene bouquet. Sent with love, peace and prayers, this simple arrangement is" +
                        " the perfect way for a funeral.\r\n" +
                        "• 5 white roses\r\n" +
                        "• 3 spiral eucalyptus\r\n" +
                        "• 4 green ball dianthus\r\n" +
                        "• 5 white spray stock\r\n" +
                        "• 3 ruscus\r\n" +
                        "• Stands approximately 16 inches tall\r\n" +
                        "• Ships in custom packaging and gift box",
                    LaunchDate = DateTime.Now.Date.AddDays(-22),
                    ColourId = 8,
                    FlowerId = 5,
                    OccasionId = 6,
                    OriginalPrice = 39.99M,
                    Discount = 0.05M,
                    Stocks = 0,
                    Sales = 334
                },
                new Bouquet
                {
                    ID = 15,
                    Name = "Purple Vanda Orchids",
                    PhotoUrl1 = "15-1.jpg",
                    PhotoUrl2 = "15-2.jpg",
                    Description = "Go exotic and forget the routine with these deep purple vanda orchids. These fresh from the farm blooms" +
                        " are good looking and full of purple power.\r\n" +
                        "• 7 Purple Vanda orchids\r\n" +
                        "• Ships in custom packaging and gift box",
                    LaunchDate = DateTime.Now.Date.AddDays(-2),
                    ColourId = 7,
                    FlowerId = 4,
                    OccasionId = 8,
                    OriginalPrice = 99.99M,
                    Discount = 0M,
                    Stocks = 150,
                    Sales = 15
                },
                new Bouquet
                {
                    ID = 16,
                    Name = "15 Darling Lavender & White Tulips",
                    PhotoUrl1 = "16-1.jpg",
                    PhotoUrl2 = "16-2.jpg",
                    Description = "The cool, ombre pattern these dainty purple, lavender and white tulips create make it a tasteful, " +
                        "high-end gift.\r\n" +
                        "• 15 Purple, Light Purple and White Tulips\r\n" +
                        "• Stands approximately 16 inches tall\r\n" +
                        "• Ships in custom packaging and gift box",
                    LaunchDate =  DateTime.Now.Date.AddDays(-37),
                    ColourId = 7,
                    FlowerId = 7,
                    OccasionId = 9,
                    OriginalPrice = 34.99M,
                    Discount = 0.40M,
                    Stocks = 7,
                    Sales = 234
                },
                new Bouquet
                {
                    ID = 17,
                    Name = "Unicorn Roses",
                    PhotoUrl1 = "17-1.jpg",
                    PhotoUrl2 = "17-2.jpg",
                    Description = "Yes, these Unicorn roses really do exist. With bright and fun colours in every petal — and a bit of " +
                        "magic — these rose spectacles will enchant any room.\r\n" +
                        "• 1 dozen long stemmed tie dyed roses\r\n" +
                        "• Stands approximately 20 inches tall\r\n" +
                        "• Ships in custom packaging and gift box",
                    LaunchDate =  DateTime.Now.Date.AddDays(-44),
                    ColourId = 1,
                    FlowerId = 5,
                    OccasionId = 12,
                    OriginalPrice = 39.99M,
                    Discount = 0M,
                    Stocks = 66,
                    Sales = 324
                }
            };

            foreach (var item in bouquets)
                context.Bouquets.Add(item);

            await context.SaveChangesAsync();
            #endregion Adding Bouquet Data

            #region Adding Sales Record Data
            var salesRecords = new SalesRecord[]
            {
                #region Relevant to Bouquet Data Whose ID Is 1
                new SalesRecord
                {
                    ID = 1,
                    SalesAmount = 49.99M * 300,
                    CreationTime = DateTimeOffset.Now.AddDays(-29.0)
                },
                new SalesRecord
                {
                    ID = 2,
                    SalesAmount = 49.99M * 255,
                    CreationTime = DateTimeOffset.Now.AddDays(-25.0)
                },
                #endregion Relevant to Bouquet Data Whose ID Is 1

                #region Relevant to Bouquet Data Whose ID Is 2
                new SalesRecord
                {
                    ID = 3,
                    SalesAmount = 39.99M * 714,
                    CreationTime = new DateTime(2019, 2, 11, 9, 0, 10, DateTimeKind.Local)
                },
                new SalesRecord
                {
                    ID = 4,
                    SalesAmount = 39.99M * (1 - 0.2M) * 200,
                    CreationTime = new DateTime(2019, 2, 13, 9, 30, 20, DateTimeKind.Local)
                },
                new SalesRecord
                {
                    ID = 5,
                    SalesAmount = 39.99M * (1 - 0.4M) * 400,
                    CreationTime = new DateTime(2019, 3, 7, 10, 0, 30, DateTimeKind.Local)
                },
                #endregion Relevant to Bouquet Data Whose ID Is 2

                #region Relevant to Bouquet Data Whose ID Is 3
                new SalesRecord
                {
                    ID = 6,
                    SalesAmount = 29.99M * 500,
                    CreationTime = DateTimeOffset.Now.AddDays(-19.0)
                },
                new SalesRecord
                {
                    ID = 7,
                    SalesAmount = 29.99M * 300,
                    CreationTime = DateTimeOffset.Now.AddDays(-17.0)
                },
                new SalesRecord
                {
                    ID = 8,
                    SalesAmount = 29.99M * 101,
                    CreationTime = DateTimeOffset.Now.AddDays(-13.0)
                },
                new SalesRecord
                {
                    ID = 9,
                    SalesAmount = 29.99M * 100,
                    CreationTime = DateTimeOffset.Now.AddDays(-5.0)
                },
                #endregion Relevant to Bouquet Data Whose ID Is 3

                #region Relevant to Bouquet Data Whose ID Is 4
                new SalesRecord
                {
                    ID = 10,
                    SalesAmount = 49.99M * 402,
                    CreationTime = DateTimeOffset.Now.AddDays(-78.0)
                },
                new SalesRecord
                {
                    ID = 11,
                    SalesAmount = 49.99M * (1 - 0.05M) * 600,
                    CreationTime = DateTimeOffset.Now.AddDays(-65.0)
                },
                #endregion Relevant To Bouquet Data Whose ID Is 4

                #region Relevant to Bouquet Data Whose ID Is 5
                new SalesRecord
                {
                    ID = 12,
                    SalesAmount = 39.99M * (1 - 0.15M) * 222,
                    CreationTime = new DateTime(2019, 4, 6, 9, 0, 10, DateTimeKind.Local)
                },
                #endregion Relevant To Bouquet Data Whose ID Is 5

                #region Relevant to Bouquet Data Whose ID Is 6
                new SalesRecord
                {
                    ID = 13,
                    SalesAmount = 69.99M * (1 - 0.35M) * 23,
                    CreationTime = DateTimeOffset.Now.AddDays(-7.0)
                },
                #endregion Relevant To Bouquet Data Whose ID Is 6

                #region Relevant to Bouquet Data Whose ID Is 7
                new SalesRecord
                {
                    ID = 14,
                    SalesAmount = 39.99M * (1 - 0.02M) * 603,
                    CreationTime = DateTimeOffset.Now.AddDays(-10.0)
                },
                new SalesRecord
                {
                    ID = 15,
                    SalesAmount = 39.99M * (1 - 0.02M) * 400,
                    CreationTime = DateTimeOffset.Now.AddDays(-3.0)
                },
                #endregion Relevant To Bouquet Data Whose ID Is 7

                #region Relevant to Bouquet Data Whose ID Is 8
                new SalesRecord
                {
                    ID = 16,
                    SalesAmount = 79.99M * (1 - 0.25M) * 604,
                    CreationTime = DateTimeOffset.Now.AddDays(-10.0)
                },
                new SalesRecord
                {
                    ID = 17,
                    SalesAmount = 79.99M * (1 - 0.25M) * 400,
                    CreationTime = DateTimeOffset.Now.AddDays(-5.0)
                },
                #endregion Relevant To Bouquet Data Whose ID Is 8

                #region Relevant to Bouquet Data Whose ID Is 9
                new SalesRecord
                {
                    ID = 18,
                    SalesAmount = 49.99M * (1 - 0.1M) * 177,
                    CreationTime = DateTimeOffset.Now.AddDays(-35.0)
                },
                new SalesRecord
                {
                    ID = 19,
                    SalesAmount = 49.99M * (1 - 0.2M) * 250,
                    CreationTime = DateTimeOffset.Now.AddDays(-25.0)
                },
                new SalesRecord
                {
                    ID = 20,
                    SalesAmount = 49.99M * (1 - 0.3M) * 350,
                    CreationTime = DateTimeOffset.Now.AddDays(-15.0)
                },
                #endregion Relevant To Bouquet Data Whose ID Is 9

                #region Relevant to Bouquet Data Whose ID Is 10
                new SalesRecord
                {
                    ID = 21,
                    SalesAmount = 29.99M * (1 - 0.1M) * 222,
                    CreationTime = DateTimeOffset.Now.AddDays(-30.0)
                },
                new SalesRecord
                {
                    ID = 22,
                    SalesAmount = 29.99M * (1 - 0.1M) * 222,
                    CreationTime = DateTimeOffset.Now.AddDays(-20.0)
                },
                new SalesRecord
                {
                    ID = 23,
                    SalesAmount = 29.99M * (1 - 0.1M) * 222,
                    CreationTime = DateTimeOffset.Now.AddDays(-10.0)
                },
                #endregion Relevant To Bouquet Data Whose ID Is 10

                #region Relevant to Bouquet Data Whose ID Is 11
                new SalesRecord
                {
                    ID = 24,
                    SalesAmount = 39.99M * (1 - 0.15M) * 222,
                    CreationTime = DateTimeOffset.Now.AddDays(-60.0)
                },
                new SalesRecord
                {
                    ID = 25,
                    SalesAmount = 39.99M * (1 - 0.15M) * 222,
                    CreationTime = DateTimeOffset.Now.AddDays(-40.0)
                },
                new SalesRecord
                {
                    ID = 26,
                    SalesAmount = 39.99M * (1 - 0.15M) * 111,
                    CreationTime = DateTimeOffset.Now.AddDays(-20.0)
                },
                #endregion Relevant To Bouquet Data Whose ID Is 11

                #region Relevant to Bouquet Data Whose ID Is 12
                new SalesRecord
                {
                    ID = 27,
                    SalesAmount = 99.99M * (1 - 0.5M) * 233,
                    CreationTime = DateTimeOffset.Now.AddDays(-3.0)
                },
                #endregion Relevant To Bouquet Data Whose ID Is 12

                #region Relevant to Bouquet Data Whose ID Is 13
                new SalesRecord
                {
                    ID = 28,
                    SalesAmount = 39.99M * (1 - 0.15M) * 230,
                    CreationTime = DateTimeOffset.Now.AddDays(-7.0)
                },
                new SalesRecord
                {
                    ID = 29,
                    SalesAmount = 39.99M * (1 - 0.15M) * 102,
                    CreationTime = DateTimeOffset.Now.AddDays(-4.0)
                },
                #endregion Relevant To Bouquet Data Whose ID Is 13

                #region Relevant to Bouquet Data Whose ID Is 14
                new SalesRecord
                {
                    ID = 30,
                    SalesAmount = 39.99M * (1 - 0.05M) * 234,
                    CreationTime = DateTimeOffset.Now.AddDays(-15.0)
                },
                new SalesRecord
                {
                    ID = 31,
                    SalesAmount = 39.99M * (1 - 0.05M) * 100,
                    CreationTime = DateTimeOffset.Now.AddDays(-5.0)
                },
                #endregion Relevant To Bouquet Data Whose ID Is 14

                #region Relevant to Bouquet Data Whose ID Is 15
                new SalesRecord
                {
                    ID = 32,
                    SalesAmount = 99.99M * 15,
                    CreationTime = DateTimeOffset.Now.AddDays(-1.0)
                },
                #endregion Relevant To Bouquet Data Whose ID Is 15

                #region Relevant to Bouquet Data Whose ID Is 16
                new SalesRecord
                {
                    ID = 33,
                    SalesAmount = 34.99M * (1 - 0.4M) * 134,
                    CreationTime = DateTimeOffset.Now.AddDays(-30.0)
                },
                new SalesRecord
                {
                    ID = 34,
                    SalesAmount = 34.99M * (1 - 0.4M) * 60,
                    CreationTime = DateTimeOffset.Now.AddDays(-20.0)
                },
                new SalesRecord
                {
                    ID = 35,
                    SalesAmount = 34.99M * (1 - 0.4M) * 40,
                    CreationTime = DateTimeOffset.Now.AddDays(-10.0)
                },
                #endregion Relevant To Bouquet Data Whose ID Is 16

                #region Relevant to Bouquet Data Whose ID is 17
                new SalesRecord
                {
                    ID = 36,
                    SalesAmount = 39.99M * 100,
                    CreationTime = DateTimeOffset.Now.AddDays(-34.0)
                },
                new SalesRecord
                {
                    ID = 37,
                    SalesAmount = 39.99M * 80,
                    CreationTime = DateTimeOffset.Now.AddDays(-24.0)
                },
                new SalesRecord
                {
                    ID = 38,
                    SalesAmount = 39.99M * 74,
                    CreationTime = DateTimeOffset.Now.AddDays(-14.0)
                },
                new SalesRecord
                {
                    ID = 39,
                    SalesAmount = 39.99M * 70,
                    CreationTime = DateTimeOffset.Now.AddDays(-4.0)
                }
                #endregion Relevant to Bouquet Data Whose ID is 17
            };

            foreach (var item in salesRecords)
                context.SalesRecords.Add(item);

            await context.SaveChangesAsync();
            #endregion Adding Sales Record Data
        } // end method InitialiseAsync
    } // end class DbInitialiser
} // end namespace NewEraFlowerStore.Data