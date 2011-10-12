require 'rubygems'
require 'albacore'

BUILD_NUMBER = ((Time.now.getutc - Time.utc(1985, 10, 8)) / 86400).to_int
SLN_FILE = "NContrib.sln"
PRODUCT = "NContib"
VERSION = "0.1.0.#{BUILD_NUMBER}"
COMPANY = "Breuer & Co. LLC"
COPYRIGHT = "Breuer & Co. LLC 2011"
COMPILE_TARGET = 'release'
BUILD_PROPERTIES = { :configuration => COMPILE_TARGET, :nowarn => "1573;1572;1591;1574" }
BUILDS_DIR = "builds"

task :default => [:build, :copy_dlls]

assemblyinfo :assemblyinfo_core  do |asm|
  asm.version = VERSION
  asm.company_name = COMPANY
  asm.product_name = PRODUCT
  asm.copyright = COPYRIGHT
  asm.title = "NContrib Core"
  asm.description = "NContrib Core"
  asm.output_file = "NContrib/Properties/AssemblyInfo.cs"
end

assemblyinfo :assemblyinfo_core4  do |asm|
  asm.version = VERSION
  asm.company_name = COMPANY
  asm.product_name = PRODUCT
  asm.copyright = COPYRIGHT
  asm.title = "NContrib4 Core"
  asm.description = "NContrib4 Core"
  asm.output_file = "NContrib4/Properties/AssemblyInfo.cs"
end

assemblyinfo :assemblyinfo_international  do |asm|
  asm.version = VERSION
  asm.company_name = COMPANY
  asm.product_name = PRODUCT
  asm.copyright = COPYRIGHT
  asm.title = "NContrib International"
  asm.description = "NContrib International"
  asm.output_file = "NContrib.International/Properties/AssemblyInfo.cs"
end

assemblyinfo :assemblyinfo_drawing  do |asm|
  asm.version = VERSION
  asm.company_name = COMPANY
  asm.product_name = PRODUCT
  asm.copyright = COPYRIGHT
  asm.title = "NContrib Drawing"
  asm.description = "NContrib Drawing"
  asm.output_file = "NContrib.Drawing/Properties/AssemblyInfo.cs"
end

task :assemblyinfo => [:assemblyinfo_core, :assemblyinfo_core4, :assemblyinfo_international, :assemblyinfo_drawing] do
	puts "Building lots of assembly files"
end

msbuild :build => :assemblyinfo do |msb|
	msb.solution = SLN_FILE
	msb.properties = BUILD_PROPERTIES
	msb.targets :clean, :build
	msb.verbosity = "minimal"
end

task :copy_dlls do
	
	collect_from = ["NContrib", "NContrib.International", "NContrib.Drawing", "NContrib4"]
	
	Dir.mkdir(BUILDS_DIR) unless Dir.exists?(BUILDS_DIR)
	
	collect_from.each do |f|
		dll_path = File.join(f, "bin", BUILD_PROPERTIES[:configuration], f + ".dll")
		FileUtils.cp(dll_path, BUILDS_DIR)
		puts dll_path
	end
end