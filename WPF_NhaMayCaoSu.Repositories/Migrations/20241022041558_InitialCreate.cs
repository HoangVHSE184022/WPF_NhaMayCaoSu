using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WPF_NhaMayCaoSu.Repository.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Boards",
                columns: table => new
                {
                    BoardId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BoardName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BoardIp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BoardMacAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BoardStatus = table.Column<int>(type: "int", nullable: false),
                    BoardMode = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Boards", x => x.BoardId);
                });

            migrationBuilder.CreateTable(
                name: "Cameras",
                columns: table => new
                {
                    CameraId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Camera1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Camera2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<short>(type: "smallint", nullable: false),
                    Time = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cameras", x => x.CameraId);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<short>(type: "smallint", nullable: false),
                    bonusPrice = table.Column<float>(type: "real", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.CustomerId);
                });

            migrationBuilder.CreateTable(
                name: "Pricing",
                columns: table => new
                {
                    PricingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Price = table.Column<float>(type: "real", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pricing", x => x.PricingId);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "RFIDs",
                columns: table => new
                {
                    RFID_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RFIDCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RFIDs", x => x.RFID_Id);
                    table.ForeignKey(
                        name: "FK_RFIDs_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<long>(type: "bigint", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.AccountId);
                    table.ForeignKey(
                        name: "FK_Accounts_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sales",
                columns: table => new
                {
                    SaleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductDensity = table.Column<float>(type: "real", nullable: true),
                    ProductWeight = table.Column<float>(type: "real", nullable: true),
                    TareWeight = table.Column<float>(type: "real", nullable: true),
                    LastEditedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<short>(type: "smallint", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalPrice = table.Column<float>(type: "real", nullable: true),
                    SalePrice = table.Column<float>(type: "real", nullable: true),
                    BonusPrice = table.Column<float>(type: "real", nullable: true),
                    RFIDCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RFID_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sales", x => x.SaleId);
                    table.ForeignKey(
                        name: "FK_Sales_RFIDs_RFID_Id",
                        column: x => x.RFID_Id,
                        principalTable: "RFIDs",
                        principalColumn: "RFID_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    ImageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImageType = table.Column<short>(type: "smallint", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SaleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.ImageId);
                    table.ForeignKey(
                        name: "FK_Images_Sales_SaleId",
                        column: x => x.SaleId,
                        principalTable: "Sales",
                        principalColumn: "SaleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Cameras",
                columns: new[] { "CameraId", "Camera1", "Camera2", "Status", "Time" },
                values: new object[] { new Guid("d70aff7e-46a3-429f-b9c1-3a69b88472cd"), "N/A", "N/A", (short)1, 30 });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "RoleName" },
                values: new object[,]
                {
                    { new Guid("b159b13c-b886-4dc2-957d-0f307625dff2"), "User" },
                    { new Guid("d4e32771-178f-4344-8252-72c5483c2014"), "Admin" }
                });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "AccountId", "AccountName", "CreatedDate", "Password", "RoleId", "Status", "Username" },
                values: new object[,]
                {
                    { new Guid("3d276f66-3d60-44d8-b052-2b2ca7ce4451"), "Standard User", new DateTime(2024, 10, 22, 4, 15, 58, 285, DateTimeKind.Utc).AddTicks(569), "$2a$11$yytt8ZRg7JF60PAvDUz11.jF.zeGsE0xucAtLlf4l4qAmWrqUvoVW", new Guid("b159b13c-b886-4dc2-957d-0f307625dff2"), 1L, "user" },
                    { new Guid("58de2995-3f93-47ed-8fb5-70e9c7ab1d4c"), "Administrator", new DateTime(2024, 10, 22, 4, 15, 58, 59, DateTimeKind.Utc).AddTicks(9042), "$2a$11$98a24ioRl.BPhPGJym/XLumWLxWPuevs03mXed3gdkW9aMHLM9Eie", new Guid("d4e32771-178f-4344-8252-72c5483c2014"), 1L, "admin" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_RoleId",
                table: "Accounts",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_Username",
                table: "Accounts",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Images_SaleId",
                table: "Images",
                column: "SaleId");

            migrationBuilder.CreateIndex(
                name: "IX_RFIDs_CustomerId",
                table: "RFIDs",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_RoleName",
                table: "Roles",
                column: "RoleName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sales_RFID_Id",
                table: "Sales",
                column: "RFID_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Boards");

            migrationBuilder.DropTable(
                name: "Cameras");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropTable(
                name: "Pricing");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Sales");

            migrationBuilder.DropTable(
                name: "RFIDs");

            migrationBuilder.DropTable(
                name: "Customers");
        }
    }
}
