#!/usr/bin/ruby

count = 0
while(line = STDIN.gets) do
  next if line.chomp == ""
  bit_b = line[0,2]
  bit_g = line[2,2]
  bit_r = line[4,2]

  hex = bit_r.hex << 10 | bit_g.hex << 5 | bit_b.hex
  hex_string = sprintf("%04x", hex)
  printf("%s %s ", hex_string[2,2], hex_string[0,2])

  count += 1
  puts if count % 8 == 0
  puts if count % 16 == 0 
end
