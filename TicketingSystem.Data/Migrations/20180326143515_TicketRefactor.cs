using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TicketingSystem.Data.Migrations
{
    public partial class TicketRefactor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TicketId",
                table: "Messages",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_TicketId",
                table: "Messages",
                column: "TicketId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Tickets_TicketId",
                table: "Messages",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Tickets_TicketId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_TicketId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "TicketId",
                table: "Messages");
        }
    }
}
