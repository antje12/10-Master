import numpy as np
import pandas as pd
import matplotlib.pyplot as plt

file_path = 'Latency.csv'
df = pd.read_csv(file_path, sep=';', header=None, names=['timestamp', 'latency'])
# Convert to datetime
df['timestamp'] = pd.to_datetime(df['timestamp'])
# Set timestamp as index
df.set_index('timestamp', inplace=True)

latency = df['latency']  # Extract the 'latency' column

print("Summary statistics for Latency:")
print("Min:", np.min(latency))
print("1st Quartile:", np.percentile(latency, 25))
print("Median:", np.median(latency))
print("Mean:", np.mean(latency))
print("3rd Quartile:", np.percentile(latency, 75))
print("Max:", np.max(latency))

# Add a horizontal line at 150 ms
plt.axhline(y=150, color='blue', linestyle='--', label='Threshold at 150 ms')

plt.boxplot([latency], labels=["Client"])
#plt.title("Latency")
#plt.xlabel("Data Sets")
plt.ylabel("Latency (ms)")
plt.show()
