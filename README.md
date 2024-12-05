<h1 align="center">PixCollect</h1>

<p align="center">
  <a href="https://github.com/Isaac987/PixCollect"><img src="Assets\pix_collect_logo.jpg" alt="PixCollect" height="120" /></a>
</p>

#

<p align="center">
  <strong>
PixCollect is a fast, multi-source image scraper designed to efficiently collect and save images for datasets and creative projects.</strong>
</p>


## Features
- Multi-threaded image scraping for fast and efficient downloads.
- Supports sources sources like Google, and Pixabay.
- Flexible configuration via appsettings.json file.
- Saves images in various formats and integrates with your file system.

## Installation
1. Clone the repository:
   ```bash
   https://github.com/Isaac987/PixCollect.git
   ```
2. Navigate to the project directory:
   ```bash
   cd PixCollect
   ```
3. Build the project:
   ```bash
   dotnet build
   ```

## Usage
### Start a Scraping Session
Run a scraping session with a specific query and limit the number of images:
```bash
dotnet run scrape run <query> <limit>
```
- `query`: The keyword or search term for the images.
- `limit`: Maximum number of images to scrape.

### Manage Image Sources
Enable or disable specific image sources for the session (currently supports: `google`, `pixabay`):
```bash
# Enable a source
dotnet run scrape enable-source <source>

# Disable a source
dotnet run scrape disable-source <source>
```
- `source`: The name of the image source to enable or disable.

### Configure Scrape Settings
View and modify default scrape settings:
```bash
# List current settings
dotnet run scrape list-settings

# Update a default setting
dotnet run scrape set-default-value <setting> <value>
```
- `setting`: The name of the setting to modify (e.g., threads, output).
- `value`: The new value for the setting.



