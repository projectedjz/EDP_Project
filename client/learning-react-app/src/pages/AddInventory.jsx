import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import {
  Box,
  Typography,
  TextField,
  Button,
  Avatar,
  Autocomplete,
  IconButton
} from "@mui/material";
import { useFormik } from "formik";
import * as yup from "yup";
import http from "../http";
import { toast } from "react-toastify";
import { ArrowBack } from "@mui/icons-material";

function AddInventory() {
  const navigate = useNavigate();
  const [products, setProducts] = useState([]);
  const [locations, setLocations] = useState([]);

  useEffect(() => {
    http
      .get("/product")
      .then((res) => setProducts(res.data))
      .catch(() => toast.error("Failed to load products"));

    http
      .get("/location")
      .then((res) => setLocations(res.data))
      .catch(() => toast.error("Failed to load locations"));
  }, []);

  const formik = useFormik({
    initialValues: {
      productId: "",
      quantity: "",
      harvestDate: "",
      expiryDate: "",
      locationId: "",
    },
    validationSchema: yup.object({
      productId: yup.number().required("Product is required"),
      quantity: yup
        .number()
        .typeError("Quantity must be a number")
        .min(1, "Quantity must be at least 1")
        .required("Quantity is required"),
      harvestDate: yup.date().nullable().required("Expiry date is required"),
      expiryDate: yup
        .date()
        .nullable()
        .required("Expiry date is required")
        .min(
          yup.ref("harvestDate"),
          "Expiry date cannot be before harvest date"
        ),
      locationId: yup.number().required("Location is required"),
    }),
    onSubmit: (data) => {
      http
        .post("/inventory", data)
        .then(() => {
          toast.success("Inventory added successfully!");
          navigate("/inventory");
        })
        .catch((err) => {
          console.error(err);
          toast.error("Failed to add inventory.");
        });
    },
  });

  return (
    <Box>
      <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
        <Typography variant="h5" sx={{ my: 2 }}>
          Add Inventory
        </Typography>
        <IconButton onClick={() => navigate(-1)}>
          <ArrowBack />
        </IconButton>
      </Box>

      <Box component="form" onSubmit={formik.handleSubmit}>
        <Autocomplete
          options={products}
          getOptionLabel={(option) =>
            `${option.productId} - ${option.productName}`
          }
          onChange={(event, value) => {
            formik.setFieldValue("productId", value ? value.productId : "");
          }}
          renderOption={(props, option) => (
            <Box
              component="li"
              {...props}
              sx={{ display: "flex", alignItems: "center", gap: 1 }}
            >
              {option.imageUrl && (
                <Avatar src={option.imageUrl} sx={{ width: 30, height: 30 }} />
              )}
              {option.productId} - {option.productName}
            </Box>
          )}
          renderInput={(params) => (
            <TextField
              {...params}
              label="Product"
              margin="dense"
              error={
                formik.touched.productId && Boolean(formik.errors.productId)
              }
              helperText={formik.touched.productId && formik.errors.productId}
            />
          )}
        />

        <TextField
          fullWidth
          margin="dense"
          label="Quantity (kg)"
          name="quantity"
          type="number"
          value={formik.values.quantity}
          onChange={formik.handleChange}
          onBlur={formik.handleBlur}
          error={formik.touched.quantity && Boolean(formik.errors.quantity)}
          helperText={formik.touched.quantity && formik.errors.quantity}
        />

        <TextField
          fullWidth
          margin="dense"
          label="Harvest Date"
          name="harvestDate"
          type="date"
          InputLabelProps={{ shrink: true }}
          value={formik.values.harvestDate}
          onChange={formik.handleChange}
          onBlur={formik.handleBlur}
          error={
            formik.touched.harvestDate && Boolean(formik.errors.harvestDate)
          }
          helperText={formik.touched.harvestDate && formik.errors.harvestDate}
        />

        <TextField
          fullWidth
          margin="dense"
          label="Expiry Date"
          name="expiryDate"
          type="date"
          InputLabelProps={{ shrink: true }}
          value={formik.values.expiryDate}
          onChange={formik.handleChange}
          onBlur={formik.handleBlur}
          error={formik.touched.expiryDate && Boolean(formik.errors.expiryDate)}
          helperText={formik.touched.expiryDate && formik.errors.expiryDate}
        />

        <Autocomplete
          options={locations}
          getOptionLabel={(option) =>
            `${option.locationId} - ${option.locationName}`
          }
          onChange={(event, value) => {
            formik.setFieldValue("locationId", value ? value.locationId : "");
          }}
          renderInput={(params) => (
            <TextField
              {...params}
              label="Location"
              margin="dense"
              error={
                formik.touched.locationId && Boolean(formik.errors.locationId)
              }
              helperText={formik.touched.locationId && formik.errors.locationId}
            />
          )}
        />

        <Box sx={{ mt: 2 }}>
          <Button variant="contained" type="submit">
            Add
          </Button>
        </Box>
      </Box>
    </Box>
  );
}

export default AddInventory;