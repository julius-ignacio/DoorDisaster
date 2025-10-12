import time
import json
import requests
from bs4 import BeautifulSoup

URL = "https://earthquake.phivolcs.dost.gov.ph/"
HEADERS = {"User-Agent": "Mozilla/5.0 (compatible; PHIVOLCS-Scraper/1.0)"}

def fetch_latest():
    resp = requests.get(URL, headers=HEADERS, timeout=15)
    resp.raise_for_status()
    return resp.text

def parse_html(html):
    soup = BeautifulSoup(html, "html.parser")

    # Find the first table that contains earthquake data
    table = soup.find("table")
    items = []

    if not table:
        return items

    # Each row (tr) after the header contains earthquake info
    for row in table.find_all("tr")[1:]:  # skip header
        cols = [td.get_text(strip=True) for td in row.find_all("td")]
        if len(cols) >= 6:
            date_time = cols[0]
            latitude = cols[1]
            longitude = cols[2]
            depth = cols[3]
            magnitude = cols[4]
            location = cols[5]

            items.append({
                "time": date_time,
                "magnitude": magnitude,
                "location": location
            })

    # Return only top 10 (latest)
    return items[:10]

if __name__ == "__main__":
    html = fetch_latest()
    data = parse_html(html)
    with open("earthquake_data.json", "w", encoding="utf-8") as f:
        json.dump(data, f, indent=2, ensure_ascii=False)
    print("Saved top 10 earthquakes to earthquake_data.json")
