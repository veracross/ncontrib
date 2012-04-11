require 'rubygems'
require 'albacore'
require 'version_bumper'

SLN_FILE = "NContrib.sln"
PRODUCT = "NContib"
COMPANY = "Breuer & Co. LLC"
COPYRIGHT = "Breuer & Co. LLC 2012"
BUILDS_DIR = "builds"

@env_buildconf = ENV['buildconf'] || 'release';

def build_properties
  { :configuration => @env_buildconf, :nowarn => "1573;1572;1591;1574" }
end

def build_version
	bumper_version.to_s
end

task :default => [:build, :copy_dlls]

assemblyinfo :assemblyinfo_core  do |asm|
  asm.version = build_version
  asm.company_name = COMPANY
  asm.product_name = PRODUCT
  asm.copyright = COPYRIGHT
  asm.title = "NContrib Core"
  asm.description = "NContrib Core"
  asm.output_file = "NContrib/Properties/AssemblyInfo.cs"
end

assemblyinfo :assemblyinfo_core4  do |asm|
  asm.version = build_version
  asm.company_name = COMPANY
  asm.product_name = PRODUCT
  asm.copyright = COPYRIGHT
  asm.title = "NContrib4 Core"
  asm.description = "NContrib4 Core"
  asm.output_file = "NContrib4/Properties/AssemblyInfo.cs"
end

assemblyinfo :assemblyinfo_international  do |asm|
  asm.version = build_version
  asm.company_name = COMPANY
  asm.product_name = PRODUCT
  asm.copyright = COPYRIGHT
  asm.title = "NContrib International"
  asm.description = "NContrib International"
  asm.output_file = "NContrib.International/Properties/AssemblyInfo.cs"
end

assemblyinfo :assemblyinfo_drawing  do |asm|
  asm.version = build_version
  asm.company_name = COMPANY
  asm.product_name = PRODUCT
  asm.copyright = COPYRIGHT
  asm.title = "NContrib Drawing"
  asm.description = "NContrib Drawing"
  asm.output_file = "NContrib.Drawing/Properties/AssemblyInfo.cs"
end

assemblyinfo :assemblyinfo_web  do |asm|
  asm.version = build_version
  asm.company_name = COMPANY
  asm.product_name = PRODUCT
  asm.copyright = COPYRIGHT
  asm.title = "NContrib Web"
  asm.description = "NContrib Web"
  asm.output_file = "NContrib.Web/Properties/AssemblyInfo.cs"
end

task :autobump do
	if @env_buildconf == 'release'
		Rake::Task["bump:revision"].invoke
	else
		Rake::Task["bump:build"].info
	end
end

task :assemblyinfo => [:assemblyinfo_core, :assemblyinfo_core4, :assemblyinfo_international, :assemblyinfo_drawing, :assemblyinfo_web] do
	puts "Building lots of assembly files"
end

msbuild :build => [:autobump, :assemblyinfo] do |msb|
  puts "Building solution with configuration: #{@env_buildconf}"
	msb.solution = SLN_FILE
	msb.properties = build_properties
	msb.targets :clean, :build
	msb.verbosity = "minimal"
end

task :copy_dlls do
	
	collect_from = ["NContrib", "NContrib.International", "NContrib.Drawing", "NContrib.Web", "NContrib4"]
	
	Dir.mkdir(BUILDS_DIR) unless Dir.exists?(BUILDS_DIR)
	
	collect_from.each do |f|
		dll_path = File.join(f, "bin", build_properties[:configuration], f + ".dll")
		FileUtils.cp(dll_path, BUILDS_DIR)
		puts dll_path
	end
end