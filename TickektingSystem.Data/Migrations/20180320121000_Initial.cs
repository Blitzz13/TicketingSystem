using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TickektingSystem.Data.Migrations
{
	public partial class Initial : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "RoleId",
				table: "Users");

			migrationBuilder.AddColumn<int>(
				name: "Role",
				table: "Users",
				nullable: false,
				defaultValue: 0);

			migrationBuilder.AddColumn<string>(
				name: "Username",
				table: "Users",
				nullable: true);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "Role",
				table: "Users");

			migrationBuilder.DropColumn(
				name: "Username",
				table: "Users");

			migrationBuilder.AddColumn<int>(
				name: "RoleId",
				table: "Users",
				nullable: false,
				defaultValue: 0);
		}
	}
}
