import os
import numpy as np
import pandas as pd
import matplotlib.pyplot as plt

directory = '6 Instances/80 Clients'

# Initialize an empty DataFrame to hold all the data
all_data = pd.DataFrame()

# Loop through each file in the directory
for filename in os.listdir(directory):
    if filename.endswith('.csv'):
        file_path = os.path.join(directory, filename)
        # Read each CSV file
        df = pd.read_csv(file_path, sep=';', header=None, names=['timestamp', 'latency'])
        # Convert 'timestamp' to datetime
        df['timestamp'] = pd.to_datetime(df['timestamp'])
        
        # Determine the last timestamp in the dataset
        last_timestamp = df['timestamp'].max()
        # Calculate the cutoff time (last 3 minutes)
        cutoff_time = last_timestamp - pd.Timedelta(minutes=3)
        # Filter the dataframe to only include data from the last 3 minutes
        df_filtered = df[df['timestamp'] >= cutoff_time]

        # Append the data from this file to the main DataFrame
        all_data = pd.concat([all_data, df_filtered])

# Set timestamp as index
all_data.set_index('timestamp', inplace=True)

latency = all_data['latency']  # Extract the 'latency' column

print("Summary statistics for Latency:")
print("Min:", np.min(latency))
print("1st Quartile:", np.percentile(latency, 25))
print("Median:", np.median(latency))
print("Mean:", np.mean(latency))
print("3rd Quartile:", np.percentile(latency, 75))
print("90% Quartile:", np.percentile(latency, 90))
print("Max:", np.max(latency))

#plt.boxplot([latency], labels=["Client"])
#plt.title("Latency")
#plt.xlabel("Data Sets")
#plt.ylabel("Latency")
#plt.show()
