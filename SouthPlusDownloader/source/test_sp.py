import requests
import sys

headers = {
    "User-Agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36",
    "Cookie": sys.argv[1]
}
url = "https://south-plus.net/thread.php?fid=227&page=1"
try:
    resp = requests.get(url, headers=headers, timeout=10)
    print(f"Status: {resp.status_code}")
    html = resp.text
    print(f"HTML Length: {len(html)}")
    if "没有权限查看" in html:
        print("Error: 没有权限查看")
    elif "cf-browser-verification" in html or "cloudflare" in html.lower():
        print("Error: Cloudflare block")
    else:
        print("Title:", html.split("<title>")[1].split("</title>")[0] if "<title>" in html else "No title")
        # print some of it
        print(html[:500])
except Exception as e:
    print("Exception:", e)
