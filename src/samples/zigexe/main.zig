// SPDX-License-Identifier: 0BSD

const std = @import("std");
const expect = std.testing.expect;
const header = @cImport(@cInclude("zigexe.h"));

test "foo bar" {
    try expect(2 + 2 == 4);
}

test "foo baz" {
    try expect('a' != 'b');
}

test "baz qux" {
    try expect(1 == 1);
}

pub fn main() void {
    std.log.info(header.HELLO_WORLD, .{ });
}
