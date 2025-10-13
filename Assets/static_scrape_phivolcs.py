import requests
from bs4 import BeautifulSoup
import json

URL = "https://earthquake.phivolcs.dost.gov.ph/"
HEADERS = {"User-Agent": "Mozilla/5.0 (compatible; PHIVOLCS-Scraper/1.0)"}

print("Fetching latest earthquake data from PHIVOLCS...")
resp = requests.get(URL, headers=HEADERS, verify=False, timeout=60)

if resp.status_code != 200:
    print("⚠️ Failed to fetch:", resp.status_code)
    exit()

soup = BeautifulSoup(resp.text, "html.parser")

# Find the main table containing the earthquake data
# Usually has 'border' or specific <th> titles like "Date/Time" or "Magnitude"
target_table = None
for table in soup.find_all("table"):
    header = table.get_text().lower()
    if "date/time" in header and "magnitude" in header:
        target_table = table
        break

if not target_table:
    print("❌ No earthquake table found in page.")
    exit()

earthquakes = []

for row in target_table.find_all("tr")[1:11]:  # skip header, get top 10
    cols = [td.get_text(strip=True) for td in row.find_all("td")]
    if len(cols) >= 6:
        earthquakes.append({
            "time": cols[0],
            "latitude": cols[1],
            "longitude": cols[2],
            "depth": cols[3],
            "magnitude": cols[4],
            "location": cols[5]
        })

if not earthquakes:
    print("⚠️ No data extracted. Table may have changed layout.")
else:
    with open("earthquake_data.json", "w", encoding="utf-8") as f:
        json.dump(earthquakes, f, indent=2, ensure_ascii=False)
    print(f"✅ Saved {len(earthquakes)} earthquake records to earthquake_data.json")
