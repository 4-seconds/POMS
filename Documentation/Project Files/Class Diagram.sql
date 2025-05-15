CREATE TABLE "User" (
  "userId" int PRIMARY KEY,
  "name" varchar,
  "email" varchar,
  "password" varchar,
  "role" varchar,
  "isActive" boolean
);

CREATE TABLE "Supplier" (
  "userId" int PRIMARY KEY,
  "businessName" varchar,
  "contactNumber" varchar,
  "address" varchar,
  "registrationStatus" varchar
);

CREATE TABLE "Admin" (
  "userId" int PRIMARY KEY
);

CREATE TABLE "GeneralManager" (
  "userId" int PRIMARY KEY
);

CREATE TABLE "ProcurementUnit" (
  "userId" int PRIMARY KEY
);

CREATE TABLE "LogisticsAndSupplyUnit" (
  "userId" int PRIMARY KEY
);

CREATE TABLE "BudgetControlUnit" (
  "userId" int PRIMARY KEY
);

CREATE TABLE "PurchaseRequest" (
  "requestId" int PRIMARY KEY,
  "createdBy" int,
  "reviewedBy" int,
  "itemDescription" varchar,
  "quantity" int,
  "unitOfMeasurement" varchar,
  "urgency" varchar,
  "status" varchar,
  "approvalComment" varchar,
  "budgetComment" text,
  "createdAt" datetime,
  "updatedAt" datetime
);

CREATE TABLE "Bid" (
  "bidId" int PRIMARY KEY,
  "placedBy" int,
  "purchaseRequest" int,
  "bidAmount" float,
  "deliveryTimeline" varchar,
  "createdAt" datetime,
  "status" varchar
);

CREATE TABLE "PurchaseOrder" (
  "orderId" int PRIMARY KEY,
  "bid" int,
  "orderDate" datetime,
  "deliveryDate" datetime,
  "status" varchar,
  "totalAmount" float
);

CREATE TABLE "GoodsReceivedNote" (
  "grnId" int PRIMARY KEY,
  "purchaseOrder" int,
  "receivedDate" datetime,
  "itemsReceived" text,
  "verifiedBy" int,
  "remarks" varchar
);

CREATE UNIQUE INDEX ON "User" ("email");

ALTER TABLE "Supplier" ADD FOREIGN KEY ("userId") REFERENCES "User" ("userId");

ALTER TABLE "Admin" ADD FOREIGN KEY ("userId") REFERENCES "User" ("userId");

ALTER TABLE "GeneralManager" ADD FOREIGN KEY ("userId") REFERENCES "User" ("userId");

ALTER TABLE "ProcurementUnit" ADD FOREIGN KEY ("userId") REFERENCES "User" ("userId");

ALTER TABLE "LogisticsAndSupplyUnit" ADD FOREIGN KEY ("userId") REFERENCES "User" ("userId");

ALTER TABLE "BudgetControlUnit" ADD FOREIGN KEY ("userId") REFERENCES "User" ("userId");

ALTER TABLE "PurchaseRequest" ADD FOREIGN KEY ("createdBy") REFERENCES "User" ("userId");

ALTER TABLE "PurchaseRequest" ADD FOREIGN KEY ("reviewedBy") REFERENCES "BudgetControlUnit" ("userId");

ALTER TABLE "Bid" ADD FOREIGN KEY ("placedBy") REFERENCES "Supplier" ("userId");

ALTER TABLE "Bid" ADD FOREIGN KEY ("purchaseRequest") REFERENCES "PurchaseRequest" ("requestId");

ALTER TABLE "PurchaseOrder" ADD FOREIGN KEY ("bid") REFERENCES "Bid" ("bidId");

ALTER TABLE "GoodsReceivedNote" ADD FOREIGN KEY ("purchaseOrder") REFERENCES "PurchaseOrder" ("orderId");

ALTER TABLE "GoodsReceivedNote" ADD FOREIGN KEY ("verifiedBy") REFERENCES "ProcurementUnit" ("userId");
