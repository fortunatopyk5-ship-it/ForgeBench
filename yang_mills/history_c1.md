# YM-HISTORY-NORM-1 — C^1 history-resummation audit (Iteration 7)

Scope:
Determine whether the history-resummation/local-cluster map is bounded C^1 between connected anchored Banach spaces. No K_0 and no STEP_0.

## 1. Typed local reconstruction correction

The Iteration-6 formula

Act_A(F)=sum_gamma Phi_gamma(F)

was not well typed when Phi_gamma(F) belongs to different local Banach spaces C_gamma.

For finite A with dependency collar A^+, define

Gamma_A={gamma: supp(gamma) intersects A^+}

and the common target

D_A^(mu)
 =
 l^1({C_gamma}_{gamma in Gamma_A}; e^{mu d(gamma)}),

with

||u||_{D_A^(mu)}
 =
 sum_{gamma in Gamma_A}
 e^{mu d(gamma)} ||u_gamma||_{C_gamma}.

The canonical embedding

iota_{gamma,A}:C_gamma -> D_A^(mu)

is the coordinate injection and has

||iota_{gamma,A}||=e^{mu d(gamma)}.

The corrected LOCAL ACTIVITY VECTOR is

Act_A(F)
 =
 sum_{gamma in Gamma_A}
 iota_{gamma,A} Phi_gamma(F).

This is NOT yet a physical reconstruction. A future synthesis/evaluation map must be introduced separately,

S_A:D_A^(mu) -> F_A,

and no boundedness claim for S_A is made here.

Under the anchored h_0^(mu),h_1^(mu) hypotheses,

||Act_A(F)||<=|A^+|h_0^(mu),

||Act_A(F)-Act_A(G)||
<=|A^+|h_1^(mu)||F-G||.

This is now a genuinely typed Banach-valued theorem.

If C_gamma are cylinder-function spaces, iota is NOT an extension of local cylinder functions to a common physical field space; it is injection into the activity direct sum. A synthesis/evaluation operator is separate.

## 2. Input and output anchored Banach spaces

Let P_in be input connected polymers eta with Banach coefficient spaces A_eta.
Let P_out be output connected polymers gamma with spaces C_gamma.

For mu_in,mu_out>=0 define

B_in(mu_in)
 =
 {F=(F_eta):
   ||F||_in
   :=
   sup_a sum_{eta contains a}
      e^{mu_in d(eta)} ||F_eta||_{A_eta}
   <infinity},

B_out(mu_out)
 =
 {K=(K_gamma):
   ||K||_out
   :=
   sup_b sum_{gamma contains b}
      e^{mu_out d(gamma)} ||K_gamma||_{C_gamma}
   <infinity}.

These are Banach spaces by the same coordinatewise/Fatou argument as YM-LOCAL-ACTIVITY-BANACH-1.

We explicitly permit

mu_out < mu_in.

The difference is decay reserve spent by localization, cluster exponentiation, collars, derivative insertions, and polymer entropy.

## 3. History-resummation map

Define abstractly

Hscr: U subset B_in(mu_in) -> B_out(mu_out),

Hscr(F)={Hscr_gamma(F)}_gamma.

This map means ONLY:

compressed/local history coefficients + internal T sums + local cluster/resummation
 ->
connected local activities/operators.

It does not include the later nonperturbative crossover RG.

For the source decomposition it is useful to separate submaps:

Hscr_T:
local analytic action/activity inputs
 ->
connected T-component operators/normalizations;

Hscr_Rcl:
pre-exponentiation connected polymer activities F(X')
 ->
post-cluster connected log activities R'(X);

and the still-unclosed outer map

Hscr_outer:
complete compressed CMP119 Omega/Lambda/T/zeta data
 ->
the product connected E/R/B/T activity coordinates.

## 4. Jacobian kernel

For a Fréchet differentiable Hscr define

M_{gamma eta}(F)
 :=
 P_gamma D Hscr_F I_eta
 in L(A_eta,C_gamma),

where I_eta is coordinate injection into B_in and P_gamma coordinate projection from B_out.

Source locality expectations:

### R'/cluster submap
For the ordinary polymer cluster map, differentiating an output connected cluster gamma with respect to input activity eta is zero unless eta occurs in a connected input family contributing to gamma. In exact-union formulations this implies supp(eta) subset supp(gamma). With enlarged localization domains one should replace this by source-specific collar containment.

### fixed T component
D_eta Hscr_{T,X}=0 unless the input activity eta meets the T-component dependency collar X^+.

### complete Balaban history map
An exact zero/collar theorem for all E/R/B/T coordinates has NOT been extracted. The source does establish component locality, but the dictionary from every input polymer eta to every output gamma is incomplete.

Thus:
Jacobian support geometry = PROVED abstractly for standard cluster/T submaps; PARTIAL for the complete source map.

## 5. Correct weighted Schur condition

For general weights

v_eta=e^{mu_in d(eta)},
u_gamma=e^{mu_out d(gamma)},

choose for every nonzero pair M_{gamma eta} a selector

s(gamma,eta) in supp(eta).

Define

S(M)
 =
 sup_b sum_a
 sup_{eta contains a}
 [
   e^{-mu_in d(eta)}
   sum_{gamma contains b: s(gamma,eta)=a}
      e^{mu_out d(gamma)}
      ||M_{gamma eta}||
 ].

YM-HISTORY-JACOBIAN-1 proves

||M||_{B_in(mu_in)->B_out(mu_out)}
<=S(M).

This is the correct orientation for the anchored norm.

A convenient quasi-local sufficient condition is

sup_{eta contains a}
 [
 e^{-mu_in d(eta)}
 sum_{gamma contains b:s(gamma,eta)=a}
 e^{mu_out d(gamma)}
 ||M_{gamma eta}||
]
<= C exp[-nu dist(a,b)].

Then

||M||
<= C C_lat(nu),

C_lat(nu)=sup_b sum_a exp[-nu dist(a,b)].

No unsupported sup_gamma sum_eta shortcut is used.

## 6. Banach-valued Cauchy alternative

The kernel estimate is stronger than needed for bounded C^1.

If Hscr is holomorphic on the larger input ball

||F||_in < R+delta

and

sup_{||F||<R+delta} ||Hscr(F)||_out <= M,

then YM-BANACH-CAUCHY-C1-1 gives

sup_{||F||<=R}
||D Hscr_F||
<= M/delta.

This is especially useful for cluster maps: strict cluster-expansion smallness reserve can replace an explicit term-by-term Jacobian count.

However it does NOT by itself imply spatial quasi-locality of the derivative kernel. For marked-source transport the Schur/off-diagonal version remains the desired endpoint.

## 7. Normalized analytic charts

The source local analytic spaces depend on

U_j^c(X,alpha_0,j,alpha_1,j),

with source-extracted

alpha_0,j
 = g_j C_0 (log g_j^-2)^{q_0},

alpha_1,j
 = g_j C_1 (log g_j^-2)^{q_1}.

Raw radii can shrink as g_j->0.

### Natural normalized candidate

In any source local complex coordinate chart in which the defining regularity conditions are represented by seminorms p_r, define the normalized gauge

||z||_{j,X}^{ref}
 =
 max_r p_r(z)/alpha_{r,j}.

An inner normalized core ||z||<=r<1 and outer analytic domain ||z||<1 would have a scale-independent normalized Cauchy collar

delta_ref=1-r.

### Problem

The source domain U_j^c is not yet proved in our notebook to be one balanced/convex ball for this Minkowski functional. It contains gauge-regularity and multiscale conditions. Moreover alpha_0,j and alpha_1,j have potentially different logarithmic powers, so a single scalar field rescaling does not automatically normalize all constraints.

The CMP119 paragraph before Theorem 1 directly indicates that new E/R/B terms live on slightly larger analytic spaces and have improved decay such as kappa -> (1+4 beta)kappa. But the exact enlarged-domain coefficients and a fixed normalized chart map have NOT been extracted.

VERDICT:
QUALITATIVE ANALYTIC RESERVE = SOURCE VERIFIED.
FIXED REFERENCE CHART WITH delta_ref>0 = NOT PROVED.

This is now a genuine blocker for a single scale-independent Banach C^1 theorem.

## 8. Source analyticity to derivative bounds

Two distinct derivatives must not be confused.

### 8.1 Derivative with respect to field/source variables
CMP119/CMP122 state analytic extension of local activities in complexified field/source domains. Once a normalized collar delta_ref is established, Cauchy gives symbolic local derivative estimates

||D_field K_X||
<= delta_ref^(-1) sup_{outer domain}||K_X||.

Thus a source activity bound

||K_X||<=A e^{-kappa d(X)}

would imply the same tree decay for first field/source derivatives, multiplied by delta_ref^(-1).

This implication is rigorous CONDITIONALLY on the normalized collar.

### 8.2 Derivative with respect to input activity coefficients
Cluster exponentiation/resummation is algebraic/analytic in its input activities. Uniform convergence on a strictly larger activity ball yields C^1 by YM-CLUSTER-CAUCHY-C1-1.

This route does not require differentiating raw hard characteristic functions: the history labels are held fixed and only analytic coefficient coordinates vary.

## 9. Loss of decay

There are two explicit sources of decay loss.

### Geometric anchored summation
If a pointwise derivative bound has exponent kappa and output norm uses mu_out, CMP109 summability requires a positive reserve

epsilon_geom=kappa-mu_out

inside the source geometric summability regime.

### CMP122-II R' exponentiation
Primary/source-verified bounds give the pre-exponentiation connected activity exponent

(1+beta) kappa

and post-exponentiation R' exponent

(1+beta/2) kappa.

Hence the cluster/log step spends exactly the source-visible reserve

epsilon_R
 =
 (beta/2) kappa.

This is a constructive example of

B_kappa
 ->
B_{kappa-epsilon}.

No equality of mu_in with the paper's raw kappa is assumed; this is a source exponent ledger.

General theorem:
if output activities decay like exp[-kappa d] and the anchored output weight is exp[(kappa-epsilon)d], then epsilon must remain in the CMP109 geometric summability regime. Polynomial collar factors are absorbed by spending an additional positive reserve epsilon_1.

## 10. Exponentiation and logarithm

For an ordinary unital Banach algebra,

D exp(K)[H]
 =
 integral_0^1
 exp((1-t)K) H exp(tK) dt,

||D exp(K)||<=exp(||K||).

For ||X||<1,

||D log(1+X)||
<=1/(1-||X||).

These are proved in YM-EXP-LOG-BANACH-1.

But an anchored polymer activity space is not automatically a Banach algebra under the relevant polymer convolution. Therefore these formulas do not by themselves prove the Balaban cluster-log map C^1.

For Balaban the safer route is:
source cluster expansion + strict convergence reserve
 -> Banach-valued holomorphic cluster map
 -> Cauchy derivative bound.

## 11. R' sector C^1 audit

CMP122-II supplies:
- a pre-exponentiation connected activity F(X') with decay
  c_1 exp[-(1+beta)kappa d(...)]
  in the source formulation;
- convergent exponentiation to R'^(k);
- R' analytic extension;
- output decay
  O(1)c_1 exp[-(1+beta/2)kappa d(...)].

At finite volume the cluster/log output is an analytic polynomial/power-series function of the input activities.

If the source smallness condition c_1<c_* is strengthened to a strict uniform margin

c_1 <= r < r+delta_cl < c_*

with c_* the source convergence threshold, then the same absolute majorant proof applies on the larger input ball and YM-CLUSTER-CAUCHY-C1-1 yields a uniform derivative bound into the weaker-decay output space.

What is source-verified:
- existence of a sufficiently-small c_1 regime and a convergent exponentiated cluster expansion;
- source decay reserve from (1+beta)kappa to (1+beta/2)kappa;
- analytic extension of the output local activity in the field/source variables.

What is not extracted:
- one explicit anchored activity convergence radius c_* in our B_in norm;
- the exact fixed-chart delta_cl after translating all source constants.

R'-SECTOR VERDICT:
CLUSTER-EXPONENTIATION SUBMAP = C^1 CONDITIONAL ON A STRICT SOURCE-TO-OUR-NORM CONVERGENCE MARGIN.
FULL compressed-history -> R' map = NOT PROVED.

This is meaningful partial progress: the failure is before/around the source-to-fixed-Banach dictionary, not in the log/exponentiation algebra itself.

## 12. T sector C^1 audit

For a fixed local history component write

T_A(F)=integral W exp(A)F,
Z_A=T_A(1),
Tbar_A=T_A/Z_A.

Exact differentiation gives

D Tbar_A[h](F)
 =
 Tbar_A(hF)-Tbar_A(h)Tbar_A(F).

CMP122-II (1.75) gives

|Tbar_A(G)|
<=C_T||G||,
C_T=exp(3 sup|sigma|).

Therefore

|D Tbar_A[h](F)|
<=
(C_T+C_T^2)||h||||F||.

For the unnormalized component,

D Z_A[h]=T_A(h),

|D Z_A[h]|
<=Z_A C_T||h||.

Combining with the source tree suppression for Z_X and a local restriction estimate on h gives the same exponential tree decay times a polynomial/collar factor. Spending an additional decay reserve absorbs that factor and CMP109 summability closes the anchored derivative sum.

T-SECTOR VERDICT:
FIXED LOCAL COMPONENT, ACTION-COEFFICIENT DERIVATIVE = PROVED.
ANCHORED SCALE-INDEXED T-SECTOR = PROVED CONDITIONAL ON THE SOURCE COLLAR/RESTRICTION DICTIONARY.
FULL compressed-history/T resummation in one fixed normalized chart = NOT PROVED.

Hard history characteristic functions are not differentiated here; history labels are fixed while analytic action/activity coefficients vary.

## 13. Source tangent versus activity tangent

A physical/source insertion is NOT automatically an element of B_in.

Introduce a source Banach space J_A and an insertion/linearization map

jmath_A:J_A -> B_in.

Only if jmath_A is bounded does a source vector J_A define a legitimate activity tangent, and only then is

D Hscr_F[jmath_A(J_A)]

well typed.

Status:
- source space J_A: TO BE CHOSEN for the observable/source class;
- insertion map jmath_A: DEFINED AS A REQUIRED INTERFACE, but boundedness is UNKNOWN;
- activity-coefficient C^1 of R' does not require solving jmath_A.

No source-mark locality theorem is claimed in Iteration 8.

## 14. Full C^1 status

To prove

Hscr:B_in(mu_in)->B_out(mu_out)

uniformly C^1 for the complete Balaban history-resummation, the missing requirements are now precise:

C1. Fixed normalized complex analytic charts for every historical-scale local coefficient space, with delta_ref>0 independent of scale/cutoff/volume.

C2. A support/collar dictionary for the complete outer CMP119 history map sufficient to define M_{gamma eta}=0 off a controlled connected neighborhood.

C3. Either:
    a quasi-local Schur estimate for M_{gamma eta},
or:
    a uniform larger-ball analytic bound for Hscr in B_out and a source-to-Banach convergence margin.

C4. Compatibility of T/zeta resummation and E/R/B extraction with these fixed charts.

None of C1-C4 is fully closed.

Hence:

FULL HISTORY C^1 STATUS:
NOT PROVED.

The leading blocker is now C1 + C2, not cluster exponentiation itself.

## 15. Exact next mathematical target

Define normalized local coefficient spaces first and prove the source chart-transition theorem

N_{j+1} o RT_j o N_j^{-1}

is analytic on one fixed reference ball with a scale-uniform collar.

Then target the Jacobian locality bound

sup_{eta contains a}
[
e^{-mu_in d(eta)}
sum_{gamma contains b}
 e^{mu_out d(gamma)}
 ||M_{gamma eta}||
]
<=
C exp[-nu dist(a,b)]

with the source-specific selector/collar convention.

If this succeeds, YM-HISTORY-JACOBIAN-QUASILOCAL-1 immediately gives a cutoff/volume/depth-uniform C^1 bound.

## 16. Final verdict

Best achieved result:

- corrected typed local reconstruction;
- exact weighted Schur theorem;
- Banach Cauchy C^1 theorem;
- explicit decay-loss ledger, including beta*kappa/2 for R' exponentiation;
- fixed T-component derivative theorem;
- R' cluster/log C^1 reduced to a strict source convergence margin.

Not achieved:

Hscr:B_kappa->B_{kappa-epsilon}
uniformly C^1 in one fixed Balaban-normalized Banach chart.

STATUS:
BLOCKED AT YM-HISTORY-NORM-1.


## 17. Graded-space alternative to a common raw field domain

A fixed Banach space of ACTIVITY COEFFICIENTS does not require all coordinates to be functions on the same raw field domain.

Define a single graded space whose coordinates are

F_{j,eta} in A_{j,eta}
 :=
 H^infinity(U^c_j(eta,alpha_0,j,alpha_1,j))

with the anchored norm taken uniformly over scale j.

YM-GRADED-ACTIVITY-BANACH-1 proves this is Banach even though the coordinate spaces differ.

Therefore there are two distinct chart questions:

(A) coefficient-space C^1:
can be formulated on the graded product, provided all restriction/localization maps between coordinate spaces are bounded;

(B) field/source C^1:
still needs a uniform analytic collar in the complex field/source variables.

This distinction prevents the shrinking raw alpha_j radii from being used as an automatic obstruction to coefficient-space history resummation.

However future RG iteration on one homogeneous chart still benefits from an explicit normalized chart-transition theorem.

## 18. Explicit R' loss-of-decay budget

Let lambda_geo be any positive reserve for which CMP109 anchored geometric summability is valid.

For the CMP122-II R' cluster step choose the activity norm exponents

mu_in
 =
(1+beta)kappa-lambda_geo,

mu_out
 =
(1+beta/2)kappa-lambda_geo.

Then

mu_in-mu_out
 =
(beta/2)kappa.

Thus the same geometric summability reserve lambda_geo is retained before and after the cluster/log map, and the entire visible source decay loss is

epsilon_R=(beta/2)kappa.

This is the cleanest source-faithful interpretation of the R' loss.

If additional support collars or polynomial factors have to be absorbed, reserve a further epsilon_col>0 and use
mu_out=(1+beta/2)kappa-lambda_geo-epsilon_col.

## 19. Quasi-local C^1 theorem specialized to exact-union cluster maps

Suppose a connected cluster/log map satisfies:
- output component bound A exp[-kappa_out d(gamma)] on a larger input ball;
- derivative support M_{gamma eta}=0 unless supp(eta) subset supp(gamma);
- a strict activity-ball margin delta>0.

Then YM-CLUSTER-JACOBIAN-SCHUR-1 applies with r_col=0.

For input anchor a in eta and output anchor b in gamma, nonzero derivative implies gamma contains both a and b. Hence

d(gamma)>=dist(a,b)

in the normalized source tree geometry.

Choose nu>0 with
kappa_out-mu_out-nu
inside the CMP109 summability regime.

Then

||D Hscr_F||
<=
(A/delta)
C_geo(kappa_out-mu_out-nu)
C_lat(nu).

This is a complete quasi-local C^1 theorem for an exact-union cluster map.

CMP122-II application status:
the cluster combinatorics has the needed connected-union architecture, but the exact source-to-our-polymer support/collar dictionary and strict anchored input radius delta are not yet fully extracted. Therefore the theorem is ready but the Yang-Mills R' instantiation remains PARTIAL.

## 20. New blocker ranking

After Iteration 7 the blockers are ranked:

1. FULL HISTORY SUPPORT/COLLAR DICTIONARY:
   identify exactly which pre-resummed input activities can influence each final E/R/B/T connected activity.

2. STRICT ACTIVITY-BALL MARGIN:
   translate source "sufficiently small" cluster assumptions into a larger anchored input ball with delta_hist>0 uniformly in scale/cutoff/volume.

3. NORMALIZED SOURCE/FIELD COLLAR:
   needed for derivatives with respect to J/U, not strictly for coefficient-space C^1 if the graded activity product is used.

4. FIXED RG CHART TRANSITION:
   needed before future iteration beyond the history map.

Thus the immediate coefficient-C^1 blocker is now (1)+(2); normalized field charts remain the immediate source-mark blocker.
