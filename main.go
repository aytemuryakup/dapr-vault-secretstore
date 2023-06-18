package main

import (
	"context"
	"encoding/json"
	"fmt"
	"io/ioutil"
	"log"
	"net/http"

	dapr "github.com/dapr/go-sdk/client"
)

// Config represents the structure of our configuration file
type Config struct {
	StoreName string `json:"storeName"`
	SecretKey string `json:"secretKey"`
}

func main() {
	// Read the configuration file
	data, err := ioutil.ReadFile("config.json")
	if err != nil {
		log.Fatalf("failed to read configuration file: %v", err)
	}

	// Unmarshal the JSON data into a Config struct
	var config Config
	if err := json.Unmarshal(data, &config); err != nil {
		log.Fatalf("failed to unmarshal configuration: %v", err)
	}

	// Create a new Dapr client
	client, err := dapr.NewClient()
	if err != nil {
		log.Fatalf("failed to create dapr client: %v", err)
	}

	defer client.Close()

	// Get the secret from the Dapr secret store
	secret, err := client.GetSecret(context.Background(), config.StoreName, config.SecretKey, nil)
	if err != nil {
		log.Fatalf("failed to get secret: %v", err)
	}

	fmt.Printf("Retrieved secret: %s = %s\n", config.SecretKey, secret[config.SecretKey])

	// Setup the server
	http.HandleFunc("/getsecret", func(w http.ResponseWriter, r *http.Request) {
		secret, err := client.GetSecret(context.Background(), config.StoreName, config.SecretKey, nil)
		if err != nil {
			http.Error(w, "Failed to get secret", http.StatusInternalServerError)
			return
		}
		secretBytes, _ := json.Marshal(secret)
		w.Write(secretBytes)
	})

	log.Fatal(http.ListenAndServe(":3000", nil))
}
