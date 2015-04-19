#!/usr/bin/env ruby

def generate_toc(max_depth)
  toc_needed= false
  @after_toc_lines = []
  while line = gets
    if (toc_needed)
      @after_toc_lines.push( line )
      begin
        if( starts_with(line, "\#") )
          heading = 1
          if( starts_with( line, "\#\#") )
            heading = 2
          end
          if( starts_with( line, "\#\#\#") )
            heading = 3
          end
          if( starts_with( line, "\#\#\#\#") )
            heading = 4
          end
          if( starts_with( line, "\#\#\#\#\#") )
            heading = 5
          end
          if( starts_with( line, "\#\#\#\#\#") )
            heading = 6
          end
          if( heading <= max_depth )
            # Remove trailing hashes
            line = line.gsub(/(^(#)+[^#]*).*/, "\\1")
            line = line.strip
            # Remove trailing spaces
            line.sub(/\s+\Z/, "")
            # Add the HTML tags
            line = line.gsub(/^######\s+/, "\t\t\t\t\t- [")
            line = line.gsub(/^#####\s+/, "\t\t\t\t- [")
            line = line.gsub(/^####\s+/, "\t\t\t- [")
            line = line.gsub(/^###\s+/, "\t\t- [")
            line = line.gsub(/^##\s+/, "\t- [")
            line = line.gsub(/^#\s+/, "- [")
            line = line.gsub(/\Z/, "][]")
            # Display matched line
            puts line
          end
        end
      rescue
        $stderr.puts "Error from => #{line}"
      end
    else
      # Search for TOC marker
      if( is_toc_marker( line ) )
        toc_needed= true
      else
        puts line
      end
    end
  end
  for i in 0..@after_toc_lines.length
    puts @after_toc_lines[i]
  end
end

def is_toc_marker(line)
  starts_with(line, "<!--TOC" )
end

def starts_with(line, value)
  if( line.length < value.length )
    return false
  end
  if( line[0,value.length] == value )
    return true
  end
  false
end

generate_toc(2)
