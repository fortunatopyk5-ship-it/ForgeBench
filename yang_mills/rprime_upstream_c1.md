# YM-RPRIME-UPSTREAM-C1 — CMP122-II Eq. (1.91) audit (Iteration 10)

Scope:
Only the upstream construction of the regular CMP122-II pre-exponentiation activity F(X'). The audited downstream map F -> R'_Bal is taken as given. No boundary B' theorem, no physical source marks, no K_0, no STEP_0.

## 1. Direct-primary transcription of CMP122-II Eq. (1.91)

Direct primary: CMP122-II printed p.388.

After the Mayer expansion and the special component decomposition, Balaban obtains

{...}
=
1+
sum_{r>=1}
sum'_{ {X'_1,...,X'_r} }
product_{p=1}^r F(X'_p).

For one connected component X', Eq. (1.91) is

F(X')
=
sum_{q>=0}
sum'_{ {X_{j_1},...,X_{j_q}} }
sum'_D
[
 product_{Y in D} int_0^1 dt(Y)
]
[
 product_{h=1}^q T'_k(X_{j_h})
]
[
 product_{Y in D} V(Y)
]
exp[
 sum_{Y in D} t(Y)V(Y)
].                                                     (U1)

Operator convention:
the product of T'_k operations acts on the coefficient/function to its right.

Here D is a subfamily of the source Mayer-localization family D_k used immediately before (1.90).

The primes on the second and third sums mean:
sum only over the chosen T-domains {X_{j_1},...,X_{j_q}} and subfamily D such that, under the special component rule described immediately before (1.90), the connected localization domain determined by these data is exactly X'.

The empty D subfamily can be admissible.

Primary p.389 further separates two cases:
- if X' contains one of the fixed large-field domains Y_i, q can start at 0;
- if X' is disjoint from union_iY_i, q starts at 1.

The present regular-lane theorem uses the second case, hence every regular contribution contains at least one T'_k factor.

The source states this explicitly in the sentence immediately after (1.91).

Earlier on printed p.387:
- the Mayer expansion is over families {X_1,...,X_n} and subfamilies D;
- each raw term determines

X'_0
=
(union_{Y in D} Y)
union
(union_{j=1}^n X_j);

- components of X'_0\(union_iY_i) are joined when they lie in one localization domain Y occurring in the product;
- the expression factorizes over the resulting components.

Thus every constituent X_{j_h} and every Y in D contributing to F(X') belongs to the connected component X'.

No E-, R-, or B-coordinate occurs explicitly in (U1).

## 2. What the two visible inputs actually are

### 2.1 T'_k(X)

CMP122-II (1.71), printed pp.378-379, defines T'_k(X) as a local integral operation.

Its exponent contains, among fixed classical/quadratic terms:
- the local residual V(X~,U_k);
- E_k(X);
- the corresponding vacuum/normalization term involving E_k(Lambda cap X);
- other fixed local terms inherited from the R/T construction.

All expressions in this integral operation are localized in X.

For coefficient calculus it is useful to split

tau_X
:=
T'_k(X)1,

bar T_X
:=
tau_X^{-1}T'_k(X).

Primary (1.74) shows tau_X is nonzero in the source analytic regime. Primary (1.75) gives

||bar T_X||_op
<=
exp(3 sup|sigma|).                                       (U2)

The source estimates |sigma| by 1 in the later (1.92) bound, so a crude source-uniform local operator constant is

C_T=e^3.                                                  (U3)

Primary (1.89), together with the sentence following it, supplies a small scalar factor for tau_X. The source then states explicitly that the improved bound has the additional term

-kappa d_k(X)

in the exponent for an arbitrary large-field-region T-operation.

Therefore in the present source normalization we may take

kappa_T=kappa.

### 2.2 V(Y)

CMP122-II printed pp.376-377 constructs V(Y) by resumming the already localized terms from the effective action.

The source says:
- for E- and R-terms the resummation includes the renormalization described earlier in CMP119 Sect. 2;
- boundary terms are summed over their localization indices;
- different types of terms are then summed;
- missing terms intersecting the large-field region are added to complete the action and subtracted from the remaining expression;
- the resulting residuals are again denoted V(Y).

Primary (1.68) gives, in the regular small-field lane,

||V(Y)||
<=
alpha
exp[-(1+2 beta)kappa d_{k,Z}(Y)],                         (U4)

with alpha arbitrarily small when the source coupling threshold gamma is sufficiently small.

For a regular component disjoint from the relevant large-field set, the relative metric reduces to the ordinary d_k metric.

The worse source bound (1.69) applies when Y itself is a component of the large-field region. These component terms do not appear as V-coordinates in the exponential used by Eq. (1.91): immediately after (1.72), the source states that terms whose localization domain equals a component of the large-field region are included into T'_k instead. Hence the V-sector entering the present regular Eq. (1.91) map is legitimately controlled by the decaying class (1.68).

Important:
CMP122-II proves the output V(Y) bounds and analyticity in the physical field variables. It does NOT, in the extracted source passage, formulate the resummation
(E,R,B,old-history coefficients) -> V
as a bounded Frechet map between coefficient Banach spaces.

This distinction is the main outcome of the present run.

## 3. Frozen-history coefficient fibers

The hard determining-set / characteristic data are NOT tangent coordinates.

Let Sigma denote one fixed admissible history/domain sector, including:
- the determining sets;
- all hard characteristic choices;
- the fixed large-field components Y_i;
- the induced families of possible localization domains;
- the special component rule used before (1.90).

Define the coefficient chart as a Banach fiber over Sigma.

The total coefficient state space relevant here is therefore a disjoint union

U_up
=
bigsqcup_{Sigma}
{Sigma} x B_up^(Sigma),                                  (U5)

not one Banach space in which Sigma is differentiated.

Within one fixed Sigma, the condition
"these raw domains determine exactly X'"
is purely combinatorial and independent of the numerical coefficient values.

Therefore:

FROZEN-HISTORY OPEN-CHART STATUS = YES
for coefficient variations inside a fixed Sigma.

No derivative across hard field-threshold/history boundaries is claimed.

## 4. Exact elementary factorization of Eq. (1.91)

For fixed Sigma and X', define the index fiber

I_Sigma(X')
=
{ I=(q, Xvec, D):
  the frozen source component rule sends (Xvec,D) exactly to X' }.

For one I define

W_D(V,t)
=
sum_{Y in D} t(Y)V_Y,                                    (U6)

P_D(V)
=
product_{Y in D}V_Y,                                     (U7)

G_D(V;t)
=
P_D(V) exp W_D(V,t).                                     (U8)

Define the ordered/local operator product

T_Xvec
=
product_{h=1}^q T'_{X_{j_h}}.                            (U9)

Then

Phi_I(T',V)
=
[
 product_{Y in D} int_0^1 dt(Y)
]
T_Xvec [ G_D(V;t) ],                                      (U10)

and

F_{X'}
=
sum_{I in I_Sigma(X')} Phi_I(T',V).                      (U11)

Thus the (1.91) map factors into:

U1 restriction to the frozen index fiber:
linear/combinatorial, no coefficient derivative;

U2 finite products V -> P_D(V):
continuous multilinear polynomial;

U3 finite sum V -> W_D(V,t):
bounded linear;

U4 exponential W -> exp W:
entire in the commutative coefficient Banach algebra;

U5 ordered product of local T' operators:
continuous multilinear polynomial;

U6 t-integration over [0,1]^D:
bounded linear, norm <=1;

U7 resummation over I_Sigma(X'):
linear after the term maps, absolutely/normally convergent in the source-majorant regime.

No separate subtraction or localization/extraction operator occurs INSIDE Eq. (1.91).
Those operations have already been absorbed into the producer of V and into T'_k.

## 5. Term-by-term first derivatives

### 5.1 V derivative

Let eta be one V-domain.

If eta not in D,

D_eta G_D(V;t)=0.

If eta in D, for h in the coefficient algebra,

D_eta G_D(V;t)[h]
=
exp(W_D)
[
  h product_{Y in D\{eta}}V_Y
  +
  t(eta)h product_{Y in D}V_Y
].                                                       (U12)

This is exact because the coefficient algebra is commutative under pointwise multiplication.

### 5.2 T' derivative

Let H_eta be a variation of T'_eta.

For the ordered operator product,

D_eta T_Xvec[H_eta]
=
sum_{r: X_{j_r}=eta}
T'_{X_{j_1}} ... T'_{X_{j_{r-1}}}
H_eta
T'_{X_{j_{r+1}}} ... T'_{X_{j_q}}.                     (U13)

In the source component family the X_{j_h} are distinct, so there is at most one summand.

### 5.3 Full term

D Phi_I
=
int_D
[
 (D T_Xvec)G_D
 +
 T_Xvec(D G_D)
].                                                       (U14)

No derivative of I_Sigma(X') appears: labels are frozen.

## 6. Local T-integral derivative

This is separate from differentiating Eq. (1.91) with respect to the already-formed operator coordinate T'_X.

Let K_X denote a local additive coefficient insertion in the exponent of the T'_k(X) integral and write abstractly

T_X(K)F
=
int e^{K}F dmu_X,

tau_X(K)=T_X(K)1,

bar T_X(K)=tau_X(K)^{-1}T_X(K).

Then

D tau_X(K)[h]
=
T_X(K)h,                                                 (U15)

and

D bar T_X(K)[h]F
=
bar T_X(K)(hF)
-
bar T_X(K)(h)bar T_X(K)(F).                              (U16)

Hence, if ||bar T_X||_op<=C_T,

||D bar T_X(K)[h]||_op
<=
(C_T+C_T^2)||h||.                                        (U17)

The unnormalized local integral satisfies directly

D T_X(K)[h]F
=
T_X(K)(hF).                                               (U18)

Thus the LOCAL exponential/integration submap inside T'_k is C^1/analytic in an open local coefficient ball on which the normalization is nonzero.

What is not yet proved is the bounded anchored producer taking all raw E/R/B/history coefficients into the local exponent insertion K_X.

## 7. Source-faithful reduced input Banach space

Eq. (1.91) itself naturally has the reduced input

B_red,k^(Sigma)
=
T_k^(Sigma)
oplus
V_k^(Sigma).                                              (U19)

Use the same global frozen coefficient algebra A_k as in the audited downstream theorem.

For T' define a mixed source-majorant/anchored norm

||T'||_T
=
max{
 sup_X ||T'_X||_op / M_T(X),

 (1/C_T,sum)
 sup_B sum_{X contains B}
 e^{mu_T d_k(X)}
 ||T'_X||_op
},                                                       (U20)

where M_T(X) is the direct-primary source majorant obtained by combining:
- the normalized-operator bound (1.75);
- the small scalar factor (1.89);
- the additional positive tree-decay exponent stated immediately after (1.89).

Write that source tree exponent as kappa rather than silently identifying it with kappa.

Thus M_T(X) has the source form

M_T(X)
=
A_T(gamma)
e^{-kappa d_k(X)},                                     (U20a)

with

A_T(gamma)
<=
e^3 exp[-2(1+beta_0)^{-1}p_0(gamma)]

up to the fixed harmless source constants already present in the local T estimate.

Choose

0<=mu_T<kappa

and define, not merely name,

C_T,sum
:=
sup_B
sum_{X contains B}
e^{mu_T d_k(X)}M_T(X)
<infinity.                                               (U20b)

The finiteness is the direct anchored geometric summability consequence of the residual exponent kappa-mu_T>0.

For V define

||V||_V
=
max{
 sup_Y ||V_Y|| / M_V(Y),

 (1/C_V,sum)
 sup_B sum_{Y contains B}
 e^{mu_V d_k(Y)}
 ||V_Y||
},                                                       (U21)

with the regular-lane source majorant

M_V(Y)
=
alpha_gamma
e^{-(1+2beta)kappa d_k(Y)}.                              (U22)

Choose

0<=mu_V<(1+2beta)kappa

and define

C_V,sum
:=
sup_B
sum_{Y contains B}
e^{mu_V d_k(Y)}M_V(Y)
<infinity.                                               (U22a)

For the regular lane this follows from CMP109 geometric summability with residual exponent

(1+2beta)kappa-mu_V>0.

Thus the sector choices are

0<=mu_T<kappa,

0<=mu_V<(1+2beta)kappa.                                  (U23)

The mixed norms serve two purposes simultaneously:
- the first term preserves the pointwise source majorants needed in the proof of (1.92)-(1.97);
- the second term supplies the requested volume-independent anchored topology.

Actual source-generated (T',V) have norm <=1 by construction of M_T,M_V,C_T,sum,C_V,sum.

The weighted-sup part is essential: an anchored ball alone would allow rare very large polymers to violate the pointwise source majorant even while retaining a finite anchored norm. The mixed norm prevents that failure.

## 8. Uniform open coefficient radius for Eq. (1.91)

The source proof of (1.92)-(1.97) is an absolute-value majorant proof.

Its smallness requirements are strict:
- alpha can be chosen arbitrarily small by shrinking the coupling threshold;
- p_0(gamma) can be made arbitrarily large;
- the displayed source inequalities before (1.97) can therefore be imposed with strict slack.

Choose the source threshold gamma so that there exists a fixed number

R_src>1                                                     (U24)

such that multiplying BOTH source majorants M_T and M_V by R_src still leaves every inequality used in (1.92)-(1.97) strict.

Then the product ball

B_{R_src}(B_red,k^(Sigma))                                (U25)

is an open coefficient neighborhood on which the same absolute majorant proof converges uniformly in volume and k.

Set the inner radius

R_in=1,

delta_up
=
R_src-1
>0.                                                       (U26)

This delta_up is cutoff/volume/RG-depth independent.

No physical U/J analytic collar is used.

## 9. Regular output projection and F space

Let Z_LF denote the frozen large-field region whose intersection defines the boundary/first-group lane at this stage.

Define

Pi_reg F
=
(F(X'): X' cap Z_LF=empty).

The C^1 theorem below is for

M_1.91^reg
:=
Pi_reg M_1.91.

If X' is regular, every constituent domain in a term determining exactly X' is contained in X', hence is also disjoint from Z_LF. Therefore this projection does not require any estimate for the forbidden boundary B' lane.

Use exactly the downstream audited regular F space:

||F||_F
=
sup_B
sum_{X' contains B}
e^{mu_F d_k(X')}
||F(X')||_{A_k},                                         (U27)

with

mu_F
=
(1+beta)kappa-lambda_F,

C_geo(lambda_F)<infinity.                                (U28)

By shrinking the fixed source threshold gamma once more, the inflated source-majorant proof on the entire R_src-ball gives

||F||_F
<=
rho_F
<
r_*^native
=
(a/9)e^{-16a-sqrt(7)theta}.                              (U29)

The exact value rho_F is not fabricated; it is the anchored sum of the inflated version of the source bound (1.97). Existence of a choice rho_F<r_* follows because alpha_gamma ->0 and p_0(gamma)->infinity as gamma->0.

## 10. Holomorphy and bounded C^1 of the exact Eq. (1.91) map

Let

M_1.91^reg:
B_red,k^(Sigma)
->
B_F(mu_F)

be defined by (U1)/(U11).

Each finite partial sum is a Banach-valued entire map in (T',V):
- products are multilinear;
- exp is entire;
- t-integration is bounded linear.

The inflated source proof gives one volume/k-uniform absolute majorant on every closed coefficient ball of radius <R_src.

Therefore, by Banach-valued normal convergence,

M_1.91:
B_{R_src}(B_red,k^(Sigma))
->
B_F(mu_F)

is holomorphic.                                                  (U30)

On the inner unit ball, Banach Cauchy gives

sup_{||(T',V)||<=1}
||D M_1.91^reg(T',V)||
<=
C_1.91,

with the explicit symbolic bound

C_1.91
=
rho_F/(R_src-1).                                         (U31)

All quantities in (U31) are fixed at the chosen source threshold and are independent of:
- total volume;
- UV cutoff;
- RG depth k.

Status:

YM-RPRIME-1.91-C1-1:
PROVED for the already-localized coefficient coordinates (T',V), under the strict source-majorant slack (U24).

No fake numerical R_src or rho_F is claimed.

## 11. Exact Jacobian support for Eq. (1.91)

Let eta be either a T'-domain or a V-domain.

From the direct-primary definition after (1.91), a term contributes to F(X') only if all its constituent localization domains determine the connected localization domain X'.

Therefore

D_eta F(X')=0
unless
eta subset X'.                                           (U32)

This is exact for the reduced coefficient variables T',V.

A deterministic selector may be chosen inside eta, so the reduced (1.91) map has

r_col=0.                                                  (U33)

Consequently, (U31) already gives the desired bounded derivative in the anchored MIXED input topology.

A stronger off-diagonal Schur estimate is NOT claimed here.

Reason:
the coordinate Cauchy kernel carries the mixed-norm injection factor w_s(eta). Exact support eta subset X' gives perfect geometric locality, but converting w_s(eta) into the orientation required by the anchored Schur test needs a marked version of the source (1.92)-(1.97) majorant. This marked two-anchor summation has not been extracted.

Thus:
- anchored operator bound: PROVED by full-Banach Cauchy;
- exact zero-collar support: PROVED;
- quantitative off-diagonal Schur kernel for the upstream map: OPEN.

## 12. Raw upstream product space

The source-faithful RAW coefficient fiber should be

B_up,k^(Sigma)
=
Tpot_k
oplus
K_E,k
oplus
K_R,k
oplus
K_B,k
oplus
K_old,k
oplus ...                                                (U34)

with sector norms

||K_s||_{mu_s}
=
sup_B
sum_{X contains B}
e^{mu_s d_k(X)}
||K_{s,X}||.                                             (U35)

Different sectors are allowed different mu_s.

The hard history Sigma is NOT a Banach coordinate.

There is a producer map

P_Sigma:
B_up,k^(Sigma)
->
B_red,k^(Sigma),

P_Sigma(raw)
=
(T'(raw),V(raw)).                                        (U36)

The full requested upstream map is

U_Sigma
=
M_1.91
circ
P_Sigma.                                                 (U37)

## 13. Producer-map audit

### 13.1 Easy/local pieces

- restriction of a coefficient to a fixed localization domain:
  linear;
- pullback by a fixed background-field substitution:
  linear in the coefficient function;
- finite sums at frozen labels:
  linear;
- local additive insertion into the T exponent:
  linear before exponentiation;
- T exponential/integration:
  analytic/C^1 by (U15)-(U18);
- normalization of T:
  C^1 by (U16) wherever tau_X !=0, with source bound (1.75).

### 13.2 First unresolved source-level operator

The construction

(E,R,B,old-history local terms)
->
V(Y)                                                     (U38)

is described on pp.376-377 as a sequence of localization, renormalization, resummation, addition and subtraction operations.

The source proves:
- physical-field analyticity of the resulting V(Y);
- the pointwise local bounds (1.68),(1.69);
- the absolute convergence of the resummations.

What is NOT yet source-extracted:
a fixed coefficient Banach chart and an operator theorem

P_V:
K_E op K_R op K_B op ...
->
V_k

with a volume/cutoff/depth-uniform bound

||D P_V||<infinity.                                      (U39)

In particular, the exact coefficient-space norm of the localization/renormalization extraction used inside this producer has not been isolated.

The same unresolved producer enters the V(X~) term inside T'_k(X).

Therefore the local T exponential is not the blocker; the raw-to-local-potential producer is.

## 13.3 CMP119 renormalization audit — lower-level chart issue, NOT current source-native blocker

Direct primary CMP119 sharpens the producer audit.

### E sector

CMP119 (2.25) writes the regular action as a sum of renormalized contributions of the form

E^(j)(Lambda_j,U)
-
E^(j)(Lambda_j,1)
-
beta_j A(phi_j,U).

CMP119 (2.26)-(2.27) then localizes E^(j) into point/localization-domain coefficients

E^(j)(X,U,z).

CMP119 (3.65) localizes the action counterterm,

A(phi_j,U)
=
sum_z A(h_z phi_j,U),

and (3.67) displays the local renormalized combination

E^(j)(Lambda_j,U,z)
-
E^(j)(Lambda_j,1,z)
-
beta_j A(h_z phi_j,U).

Vacuum subtraction

f -> f-f(1)

is bounded linear in the local H-infinity coefficient norm, with operator norm at most 2.

The nontrivial operation is the relevant/coupling extraction producing beta_j.

CMP119 (3.64) identifies beta_j with a second-derivative coefficient of the localized quadratic kernel (equivalently a second derivative of its Fourier transform at zero). Thus, at coefficient level, the E-renormalization is a LINEAR projection of the schematic form

P_E f
=
f-f(1)-ell_beta,j(f) A_j,                                (U39a)

where ell_beta,j is a linear second-order extraction functional.

To prove a cutoff/scale-uniform bound for P_E one needs

sup_j ||ell_beta,j||_{A_{E,j}->C}<infinity               (U39b)

in the chosen scale-graded coefficient norm, together with a uniform bound for the corresponding localized action atom A_j.

The native H-infinity domains have scale-dependent radii alpha_0,j,alpha_1,j. A naive unweighted Cauchy estimate for ell_beta,j may therefore lose powers of those shrinking radii. This is precisely where a normalized/derivative-weighted coefficient chart is required.

No uniform bound (U39b) has yet been extracted.

### R sector

CMP119 (2.30) gives

R_k(U)
=
sum_j
[
 R^(j)(Lambda_j,U)
 -
 R^(j)(Lambda_j,1)
].

No coupling counterterm is needed; the source explicitly says only vacuum-energy renormalization is performed.

Thus the local coefficient renormalization

P_R f=f-f(1)

is bounded linear with norm <=2 in H-infinity.

The localized R coefficients have the strong exponential bound (2.31), with arbitrarily large source localization exponent after fixing parameters.

### B sector

CMP119 states immediately after (2.33) that B_k needs no renormalization. It is already a sum of bounded/small localized analytic terms, represented in (2.40)-(2.42).

Thus, after freezing the history/domain labels, the B contribution to the pp.376-377 producer consists only of restriction/reindexing/resummation, not a relevant-part extraction.

### Consequence

The broad blocker P_V can now be sharpened.

The FIRST genuinely unproved analytic submap is

ell_beta,j:
E-local coefficient -> coupling/relevant coefficient,

uniformly in the scale-graded native analytic topology.

If beta_j is treated as an already-extracted separate source coordinate, then the subsequent local E renormalization is bounded linear, just like R and B. However this run does NOT use that convention to claim the raw theorem: the requested raw E-coordinate map must justify the extraction rather than freeze it silently.

So the exact blocker is

YM-E-RELEVANT-EXTRACT-1:
prove a scale-uniform bounded extraction/projection theorem for (3.64)-(3.67).

Only after that should the remaining linear pp.376-377 resummation be packaged as a bounded anchored incidence operator.

## 14. Pointwise derivative status

For a reduced input sector s in {T,V}, let I_{s,eta} be the coordinate injection. In the mixed norm,

w_s(eta)
:=
||I_{s,eta}||_{A_eta -> B_red}

is explicit:

w_T(eta)
=
max{
 M_T(eta)^{-1},
 C_T,sum^{-1}e^{mu_T d_k(eta)}
},                                                       (U40a)

w_V(eta)
=
max{
 M_V(eta)^{-1},
 C_V,sum^{-1}e^{mu_V d_k(eta)}
}.                                                       (U40b)

Let the inflated source-majorant proof give the component estimate on the outer ball

||F_{X'}||
<=
A_F
e^{-kappa_F d_k(X')}                                    (U40c)

with kappa_F>mu_F.

Cauchy in the FULL reduced Banach space, with margin delta_up=R_src-1, gives

||D_{s,eta}F_{X'}[h]||
<=
(A_F/delta_up)
w_s(eta)
e^{-kappa_F d_k(X')}
||h||.                                                   (U40)

Together with the exact support rule eta subset X', this is the source-faithful multi-sector version of the requested pointwise Jacobian estimate.

No unspecified polynomial factor is hidden here: any source combinatorial/polynomial loss is already included in the choice of the outer-ball pointwise majorant (U40c), obtained by rerunning the absolute source estimate with strict slack.

Status:
PROVED as a pointwise Cauchy kernel bound for the reduced coordinates T',V.

It is sufficient for the global mixed-Banach operator estimate (U31), but a separate marked summation is still required for an off-diagonal Schur theorem.

For raw E/R/B coordinates the corresponding bound is CONDITIONAL on the missing producer estimate (U39).

## 15. Composition with the audited downstream R' map

The downstream theorem gives

H_R':
B_F
->
B_R'

with

sup ||D H_R'||<=C_R'.                                    (U41)

Therefore, for reduced upstream coordinates,

H_R'
circ
M_1.91^reg

is bounded C^1 and

||D(H_R' circ M_1.91)||
<=
C_R' C_1.91.                                             (U42)

Status:
PROVED for (T',V) coefficient coordinates.

For the full raw map,

H_R'
circ
U_Sigma
=
H_R'
circ
M_1.91^reg
circ
P_Sigma,

one would have

||D(H_R' circ U_Sigma)||
<=
C_R' C_1.91 C_P,                                         (U43)

but C_P has not been proved.

## 16. YM-RPRIME-UPSTREAM-C1-1 verdict — INTERMEDIATE / SUPERSEDED BY SECTS. 18-25

Desired raw theorem:
(T,E,R,B,local history coefficients) -> F is uniformly bounded C^1.

Status:
PARTIAL / BLOCKED.

What IS proved:
the exact CMP122-II (1.91) resummation map from already-localized coefficient coordinates

(T',V) -> F

is uniformly Banach-holomorphic/C^1 in a frozen-history anchored coefficient chart with a cutoff/volume/depth-independent coefficient radius.

What is NOT proved:
the producer map from raw E/R/B/history coefficient families into the local residual V(Y) and the local exponent data entering T'_k(X).

FIRST FAILED SUBMAP:

YM-E-RELEVANT-EXTRACT-1,

the scale-uniform bounded relevant/coupling extraction inside the E-sector renormalization (CMP119 (3.64)-(3.67)).

The R vacuum subtraction is bounded linear, and B needs no renormalization. The remaining pp.376-377 resummation is linear at frozen labels once its renormalized local atoms are supplied, but its anchored incidence constant should still be written explicitly after YM-E-RELEVANT-EXTRACT-1 is closed.

This is now the exact blocker.

## 17. Non-claims

No boundary B' lane.
No full CMP119 outer-history C^1 theorem.
No physical source derivative.
No D^2 theorem.
No K_0.
No STEP_0.


## 18. Final correction: use the source-native RENORMALIZED E/R/B coordinates

The temporary blocker YM-E-RELEVANT-EXTRACT-1 arose only if one insists on using a pre-renormalization E coefficient as the upstream Banach coordinate.

That is not the source-native state coordinate at this stage.

CMP119 printed p.259 states explicitly:
- E_k is the fully renormalized regular part of the current effective action;
- vacuum-energy and coupling-constant counterterms are already included in it;
- R_k has already undergone vacuum-energy renormalization;
- B_k requires no renormalization.

Therefore the coefficient chart immediately upstream of CMP122-II p.376 should use:

E_ren = already-renormalized local E atoms,
R_ren = already vacuum-renormalized local R atoms,
B_loc = already-localized B atoms.

The extraction (3.64) belongs to the EARLIER chart transition that constructs E_ren. It is not part of the current map from the source inductive state to Eq. (1.91).

Thus YM-E-RELEVANT-EXTRACT-1 remains a legitimate question about a lower-level UNRENORMALIZED chart, but it is NOT a blocker for the source-native upstream theorem requested here.

## 19. Source-native raw input Banach space

For one frozen history sector Sigma define

B_up,k^(Sigma)
=
T_k
direct-sum E_ren,k
direct-sum R_ren,k
direct-sum B_loc,k
direct-sum K_aux,k.                                      (U44)

K_aux contains any remaining already-localized source atoms entering the p.376-377 resummation; it does not contain hard characteristic labels.

Each local atom lambda has:
- a localization support X_lambda;
- a source pointwise majorant M_s(lambda) from the corresponding CMP119/CMP122 inductive bound.

For each sector s define the mixed norm

||K_s||_s
=
max{
 sup_lambda ||K_{s,lambda}||/M_s(lambda),

 (1/C_s)
 sup_B
 sum_{lambda: B in X_lambda}
 e^{mu_s d(X_lambda)}
 ||K_{s,lambda}||
}.                                                       (U45)

Here C_s is the finite anchored sum of the source majorant at the chosen residual decay exponent.

For the T sector this is the audited T' norm of Sect. 7, with the exact source tree exponent kappa after (1.89).

For E_ren, R_ren and B_loc, choose mu_s strictly below the source localization exponent of that sector. CMP119 gives:
- exponential localization for local E coefficients;
- arbitrarily strong exponential localization for R in (2.31);
- exponential localization for B in (2.42).

Use the max product norm on B_up.

Actual source-native coefficient families have norm <=1 after fixing the harmless sector normalization constants C_s.

This is an anchored coefficient topology; it is not the physical-field H-infinity topology alone.

## 20. The p.376-377 V-producer is a bounded linear incidence map in this chart

Freeze Sigma.

Every operation used on pp.376-377 after the local atoms have been renormalized is linear in their coefficient values:
- restriction to an admissible localization domain;
- summation over auxiliary domains Y'_i;
- summation over localization domains X;
- summation over j;
- summation over term types;
- exceptional-term resummation;
- addition of missing action terms and subtraction from the residual.

Thus there is a signed incidence kernel

A_Sigma(Y,lambda)

such that

V(Y)
=
sum_lambda
A_Sigma(Y,lambda)K_lambda.                               (U46)

The kernel is combinatorial/geometric and coefficient-independent on the frozen history fiber.

The absolute-value proof on p.376 states that the resummations are bounded by a constant times the same extracted exponential factor; p.377 completes the remaining resummations and obtains (1.68).

Therefore, if M_lambda denotes the exact source majorant assigned before the resummation, the proof yields a cutoff/volume/k-uniform finite constant C_res with

sum_lambda
|A_Sigma(Y,lambda)| M_lambda
<=
C_res M_V(Y),                                            (U47)

for the decaying V-class entering the exponential in (1.72).

Absorb C_res into the harmless normalization of M_V. Then

||P_V K||_V
<=
||K||_up.                                                (U48)

The anchored part follows by summing M_V(Y) with CMP109 geometric summability.

Important source separation:
terms whose localization domain is exactly a component of the large-field region obey the worse (1.69), but immediately after (1.72) the source states that these terms are INCLUDED INTO T'_k and omitted from the V exponential. Hence (U47)-(U48) use only the (1.68) V-class and do not invoke the forbidden boundary B' theorem.

Status:

P_V:
E_ren direct-sum R_ren direct-sum B_loc direct-sum K_aux
->
V

is BOUNDED LINEAR on the frozen-history source-native coefficient space.

## 21. Source-native producer P_Sigma

Define

P_Sigma:
B_up,k^(Sigma)
->
B_red,k^(Sigma)=T_k direct-sum V_k

by

P_Sigma(T',E_ren,R_ren,B_loc,K_aux)
=
(T', P_V(E_ren,R_ren,B_loc,K_aux)).                      (U49)

With the normalized max product norms,

||P_Sigma||<=1.                                          (U50)

Thus P_Sigma is entire and its derivative is itself.

The physical source subset may impose additional compatibility identities between T', E_ren, V and the hard history data. Those identities do not obstruct the existence of this ambient coefficient map.

No derivative across hard history sectors is taken.

## 22. Final YM-RPRIME-UPSTREAM-C1-1 theorem

Let

U_Sigma
=
M_1.91^reg
circ
P_Sigma.                                                 (U51)

By YM-RPRIME-191-C1-1 and (U50),

U_Sigma:
B_1(B_up,k^(Sigma))
->
B_F(mu_F)

is uniformly C^1; in fact it is the restriction of a holomorphic map to the source-native open neighborhood inherited from R_src.

Moreover

sup_{||K||_up<=1}
||D U_Sigma(K)||
<=
C_U,

with

C_U
<=
C_1.91 ||P_Sigma||
<=
C_1.91
=
rho_F/(R_src-1).                                         (U52)

All constants are independent of:
- total volume;
- UV cutoff;
- RG depth k.

Status:

YM-RPRIME-UPSTREAM-C1-1:
PROVED IN THE SOURCE-NATIVE RENORMALIZED COEFFICIENT CHART.

This is exactly the chart of the current CMP119 inductive state:
T' plus fully-renormalized E, vacuum-renormalized R, and localized B/auxiliary atoms.

Non-claim:
C^1 of the LOWER-LEVEL chart transition from unrenormalized E_raw to E_ren is not proved here and is not part of Eq. (1.91) upstream map.

## 23. Raw-source Jacobian support

For a T' coordinate eta,

D_eta U_Sigma,X'=0

unless

eta subset X'.                                           (U53)

For an E_ren/R_ren/B_loc/auxiliary atom lambda, define the frozen source incidence set

Out_Sigma(lambda)
=
{Y:
 A_Sigma(Y,lambda) !=0}.                                  (U54)

Then exactly

D_lambda U_Sigma,X'=0

unless there exists

Y in Out_Sigma(lambda)

with

Y subset X'.                                             (U55)

This is the correct source-faithful raw support rule.

No fixed metric collar for lambda relative to X' is inferred from (U55). Extracting a two-anchor decay estimate for A_Sigma is the separate marked-Schur problem.

Thus:
- exact combinatorial support: PROVED in incidence form;
- uniform bounded derivative: PROVED;
- quantitative off-diagonal Schur kernel: still OPEN.

## 23.1 Raw-coordinate pointwise Cauchy bound

For a source-native raw coordinate lambda in sector s, let

I_{s,lambda}:A_{s,lambda}->B_up

be its coordinate injection and define

w_up(lambda)
=
||I_{s,lambda}||.

From the mixed norm (U45),

w_up(lambda)
=
max{
 M_s(lambda)^(-1),
 C_s^(-1)e^{mu_s d(X_lambda)}
}.                                                       (U55a)

On the outer source-native coefficient ball, the same inflated absolute majorant used for (1.97) gives a component estimate

||U_Sigma(K)_{X'}||
<=
A_F e^{-kappa_F d_k(X')}.                               (U55b)

Let delta_up>0 be the distance from the inner source ball to the chosen outer holomorphy ball. Full Banach-space Cauchy gives

||D_{s,lambda}U_Sigma(K)_{X'}[h]||
<=
(A_F/delta_up)
w_up(lambda)
e^{-kappa_F d_k(X')}
||h||.                                                   (U55c)

This is the requested source-faithful multi-sector pointwise derivative bound.

Together with (U55), it has both:
- an analytic amplitude estimate;
- the exact frozen-incidence zero rule.

It still does NOT by itself yield a two-anchor Schur sum, because w_up(lambda) must be combined with the geometry of Out_Sigma(lambda).

## 24. Composition with audited native R'

The accepted downstream theorem gives

H_R':
B_F
->
B_R',

sup||D H_R'||<=C_R'.

Therefore

H_R'
circ
U_Sigma

is uniformly C^1 and

boxed:
sup
||D(H_R' circ U_Sigma)||
<=
C_R' C_U
<=
C_R' C_1.91.                                             (U56)

This closes the requested regular source-native chain

(T',E_ren,R_ren,B_loc,local frozen history data)
->
F_reg
->
R'_Bal.

No D^2 statement is made.

## 25. Final Iteration-10 status

PROVED:
- exact Eq. (1.91) transcription;
- frozen-history open coefficient chart;
- source-native product Banach space;
- bounded linear V-producer after using the already-renormalized CMP119 state coordinates;
- regular (T',V)->F holomorphy/C^1;
- full source-native upstream bounded C^1;
- composition with audited R'_Bal bounded C^1.

OPEN:
- off-diagonal/marked Schur kernel for the upstream producer;
- lower-level unrenormalized E_raw -> E_ren chart transition;
- any boundary B' theorem;
- physical source insertion;
- K_0;
- STEP_0.
