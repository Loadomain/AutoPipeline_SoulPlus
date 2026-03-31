import requests

cookie = "_ga=GA1.1.1191919142.1774710674; eb9e6_cknum=UlJWVg5SUQFTBT5oBwEAUlFUBlFTV19VBQkGVlRUAFYMAgdWU11ZVw1aUQ4%3D; eb9e6_ck_info=%2F%09; eb9e6_winduser=UFNRVwhbaFpdB1VYVw0DBgRSAgMHAVhVUwhWAABQAAICAFZUBQYAaw%3D%3D; peacemaker=1; eb9e6_threadlog=%2C107%2C216%2C4%2C226%2C227%2C; eb9e6_readlog=%2C2527865%2C2798717%2C2793546%2C2801091%2C; cf_clearance=T7F0OMlcwUxkpJMcpQeLTXBuEek4N5wSuKY8vQrmS_E-1774766844-1.2.1.1-kHggZNtwD9prUJphY.94.fLG82YyQpxRhBj00myK4ceAjDgCINdHQjQyftrG2dVdhvyVHJt5zQkRPmISdPhwoHIDzt9IuW3kzQqCSq.aHSHxPNdWN57b4VxMFyeQx5JeuE8nNb6wfE00ShyByucJyhLcyC6zbTFBoF04IHENxHv8JGwoDXGyrR0XpB6oQpWIClTEdg2whMBHk1MDIKsqp1y4VZrAVCiO_RWhdqXolOA; eb9e6_ol_offset=294880; eb9e6_lastpos=other; eb9e6_lastvisit=2457%091774767051%09%2Fu.php%3F; _ga_KYN9Z1NWGY=GS2.1.s1774764589$o2$g1$t1774767053$j59$l0$h0"
ua1 = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36"
ua2 = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Safari/537.36"
ua3 = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0.0.0 Safari/537.36"

url = "https://south-plus.net/thread.php?fid-227-page-1.html"

for ua in [ua1, ua2, ua3]:
    headers = {
        "User-Agent": ua,
        "Cookie": cookie
    }
    resp = requests.get(url, headers=headers, timeout=5)
    print(f"Testing UA: {ua}")
    print(f"Status: {resp.status_code}")
    if "只有注册会员" in resp.text or "正规版块" in resp.text:
       print("Failed: Only members.")
    elif "登录" in resp.text and "退出" not in resp.text:
       print("Failed: Not logged in.")
    else:
       print("Success! Matches this UA or Cookie is valid!")
    print("-" * 30)

