import requests

url = "https://south-plus.net/read.php?tid-2514275.html"
cookie = "_ga=GA1.1.1191919142.1774710674; eb9e6_cknum=UlJWVg5SUQFTBT5oBwEAUlFUBlFTV19VBQkGVlRUAFYMAgdWU11ZVw1aUQ4%3D; eb9e6_ck_info=%2F%09; eb9e6_winduser=UFNRVwhbaFpdB1VYVw0DBgRSAgMHAVhVUwhWAABQAAICAFZUBQYAaw%3D%3D; peacemaker=1; eb9e6_threadlog=%2C107%2C216%2C4%2C226%2C227%2C; eb9e6_readlog=%2C2527865%2C2798717%2C2793546%2C2801091%2C; cf_clearance=T7F0OMlcwUxkpJMcpQeLTXBuEek4N5wSuKY8vQrmS_E-1774766844-1.2.1.1-kHggZNtwD9prUJphY.94.fLG82YyQpxRhBj00myK4ceAjDgCINdHQjQyftrG2dVdhvyVHJt5zQkRPmISdPhwoHIDzt9IuW3kzQqCSq.aHSHxPNdWN57b4VxMFyeQx5JeuE8nNb6wfE00ShyByucJyhLcyC6zbTFBoF04IHENxHv8JGwoDXGyrR0XpB6oQpWIClTEdg2whMBHk1MDIKsqp1y4VZrAVCiO_RWhdqXolOA; eb9e6_ol_offset=294880; eb9e6_lastpos=other; eb9e6_lastvisit=2457%091774767051%09%2Fu.php%3F; _ga_KYN9Z1NWGY=GS2.1.s1774764589$o2$g1$t1774767053$j59$l0$h0"

headers = {
    "User-Agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36",
    "Cookie": cookie
}

resp = requests.get(url, headers=headers, timeout=5)
with open('debug_test_read.txt', 'w', encoding='utf-8') as f:
    f.write(resp.text)
print("Saved to debug_test_read.txt. Size:", len(resp.text))
