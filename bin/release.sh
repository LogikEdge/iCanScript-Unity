#!/usr/bin/env ruby
require 'optparse'
require 'ostruct'

# Option Parser
options= {}
opts = OptionParser.new
  
opts.on("-M ARG", "--Major ARG", Integer) { |val| options['major']= val }

my_argv= ["-M", "4"]
remaining= opts.parse(*my_argv)
puts "Reconized options: #{options}"
puts "Remaining options: #{remaining.join(', ')}"
